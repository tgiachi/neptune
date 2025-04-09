using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CommandLine;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Neptune.Core.Extensions;
using Neptune.Core.Utils;
using Neptune.Database.Core.Extensions;
using Neptune.Database.Core.Interfaces.Services;
using Neptune.Rest.Server.Data.Options;
using Neptune.Rest.Server.Entities;
using Neptune.Rest.Server.Hosted;
using Neptune.Rest.Server.Interfaces;
using Neptune.Rest.Server.Modules;
using Neptune.Rest.Server.Routes;
using Neptune.Rest.Server.Services;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Directories;
using Neptune.Server.Core.Data.Rest;
using Neptune.Server.Core.Data.Rest.Validation;
using Neptune.Server.Core.Extensions;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Server.Core.Types;
using Neptune.Transport.Core.Extensions;
using Neptune.Udp.Transport;
using Neptune.Udp.Transport.Config;
using Serilog;
using Serilog.Formatting.Json;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Neptune.Rest.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var options = Parser.Default.ParseArguments<NeptuneOptions>(args);

        if (Environment.GetEnvironmentVariable("NEPTUNE_ROOT_DIRECTORY") != null)
        {
            options = options.WithParsed(
                o => o.RootDirectory = Environment.GetEnvironmentVariable("NEPTUNE_ROOT_DIRECTORY")
            );
        }

        if (string.IsNullOrEmpty(options.Value.RootDirectory))
        {
            options.Value.RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "neptune_server");
        }

        if (options.Errors.Any())
        {
            Console.WriteLine("Invalid arguments. Please provide the root directory.");
            Environment.Exit(1);
            return;
        }

        if (options.Value.ShowHeader)
        {
            ShowHeader();
        }

        var rootDirectory = options.Value.RootDirectory;

        var directoriesConfig = new DirectoriesConfig(rootDirectory);

        var config = await LoadConfigAsync(directoriesConfig.Root, "neptune_server.yaml");

        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddSingleton(config);

        var loggingConfiguration = new LoggerConfiguration().WriteTo.Console()
            .WriteTo.File(
                formatter: new JsonFormatter(),
                Path.Combine(directoriesConfig[DirectoryType.Logs], "neptune_server_.log"),
                rollingInterval: RollingInterval.Day
            );


        loggingConfiguration.MinimumLevel.Is(config.LogLevel.ToLogEventLevel());


        Log.Logger = loggingConfiguration.CreateLogger();


        builder.Logging.ClearProviders().AddSerilog();

        builder.Services.AddNeptuneModule<AbyssSignalModule>();

        // Add services to the container.
        builder.Services.AddAuthorization();


        builder.WebHost.UseKestrel(
            s =>
            {
                s.AddServerHeader = false;
                s.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
                s.Listen(
                    new IPEndPoint(config.WebServer.Host.ToIpAddress(), config.WebServer.Port),
                    o => { o.Protocols = HttpProtocols.Http1; }
                );
            }
        );


        if (config.Development.EnableSwagger)
        {
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen(
                opt =>
                {
                    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Neptune Server", Version = "v1" });
                    opt.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Please enter token",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "bearer"
                        }
                    );

                    opt.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new string[] { }
                            }
                        }
                    );
                }
            );
        }

        builder.Services.AddFluentValidationAutoValidation();

        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();


        builder.Services.AddProblemDetails();

        var messageQueueConfig = config.MessagesQueue.ParseMessageQueueConnection();

        if (messageQueueConfig.Type == MessageQueueType.Internal)
        {
            builder.Services.AddSingleton<IMessageQueueService, InternalMessageQueueService>();
        }

        if (messageQueueConfig.Type == MessageQueueType.RabbitMQ)
        {
            builder.Services.AddSingleton<IMessageQueueService, RabbitMqMessageQueueService>();
        }

        builder.Services.RegisterServiceToLoadAtStartup<IMessageQueueService>();


        builder.Services.ConfigureHttpJsonOptions(
            o =>
            {
                o.SerializerOptions.DefaultIgnoreCondition = JsonUtils.GetDefaultJsonSettings().DefaultIgnoreCondition;
                o.SerializerOptions.Converters.Add(JsonUtils.GetDefaultJsonSettings().Converters[0]);
                o.SerializerOptions.WriteIndented = JsonUtils.GetDefaultJsonSettings().WriteIndented;
                o.SerializerOptions.PropertyNamingPolicy =
                    JsonUtils.GetDefaultJsonSettings().PropertyNamingPolicy;

                o.SerializerOptions.PropertyNameCaseInsensitive =
                    JsonUtils.GetDefaultJsonSettings().PropertyNameCaseInsensitive;
            }
        );


        builder.Services
            .AddDbEntity<MessageEntity>()
            .AddDbEntity<AuditLogEntity>()
            .AddDbEntity<ChannelEntity>()
            .AddDbEntity<ChannelMemberEntity>()
            .AddDbEntity<UserEntity>();

        builder.Services.RegisterDatabase(
            config.Database.ConnectionString.ParseDbConnectionString(),
            config.Database.EnableSqlLogging
        );


        InitJwtAuth(builder.Services, config);

        builder.Services.AddSingleton<ITransportManagerService, TransportManagerService>();

        builder.Services

            .RegisterServiceToLoadAtStartup<IDatabaseService>()
            .RegisterServiceToLoadAtStartup<ITransportManagerService>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IMessageService, MessageService>();

        builder.Services.AddHostedService<NeptuneHostedService>();

        builder.Services.AddNeptuneTransport<UdpTransport,UdpTransportConfig>(directoriesConfig);

        var app = builder.Build();


        var version = Assembly.GetExecutingAssembly().GetName().Version;
        // Default endpoint
        app.MapGet(
                "/",
                () => Results.Ok(new VersionResponseObject("Neptune Server", version.ToString(), "OK"))
            )
            .WithName("GetVersion")
            .Produces<VersionResponseObject>(StatusCodes.Status200OK)
            .WithTags("Info");

        // Configure the HTTP request pipeline.
        if (config.Development.EnableSwagger)
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neptune v1"));

            Log.Logger.Information(
                "!!! OpenAPI is enabled. You can access the documentation at http://{Hostname}:{Port}/swagger",
                config.WebServer.Host,
                config.WebServer.Port
            );
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        var apiGroup = app.MapGroup("/api/v1");

        apiGroup
            .MapGenerateKeysRoutes()
            .MapSystemRoutes()
            .MapMessageRoutes()
            .MapAuthRoutes()
            ;

        await app.RunAsync();
    }

    private static async Task<NeptuneServerConfig> LoadConfigAsync(string rootDirectory, string configFileName)
    {
        var configFile = Path.Combine(rootDirectory, configFileName);

        if (!File.Exists(configFile))
        {
            Log.Warning("Configuration file not found. Creating default configuration file...");

            var config = new NeptuneServerConfig();

            await File.WriteAllTextAsync(configFile, config.ToYaml());
        }

        Log.Logger.Information("Loading configuration file...");

        var loadedConfig = (await File.ReadAllTextAsync(configFile)).FromYaml<NeptuneServerConfig>();

        await File.WriteAllTextAsync(configFile, loadedConfig.ToYaml());

        return loadedConfig;

    }

    private static void InitJwtAuth(IServiceCollection services, NeptuneServerConfig config)
    {
        IdentityModelEventSource.ShowPII = true;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config.JwtAuth.Issuer,
                        ValidAudience = config.JwtAuth.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JwtAuth.Secret)),
                    };
                }
            );

        services.AddAuthorization();
    }

    private static void ShowHeader()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "Neptune.Rest.Server.Assets.header.txt";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var version = assembly.GetName().Version;

        var customAttribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "Codename");

        Console.WriteLine(reader.ReadToEnd());
        Console.WriteLine($"  >> Codename: {customAttribute?.Value ?? "Unknown"}");
        Console.WriteLine($"  >> Version: {version}");
    }
}
