using System.Reflection;
using CommandLine;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Neptune.Core.Extensions;
using Neptune.Database.Core.Extensions;
using Neptune.Database.Core.Interfaces.Services;
using Neptune.Rest.Server.Data.Options;
using Neptune.Rest.Server.Entities;
using Neptune.Rest.Server.Hosted;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Directories;
using Neptune.Server.Core.Extensions;
using Neptune.Server.Core.Types;
using Serilog;
using Serilog.Formatting.Json;

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
        // Add services to the container.
        builder.Services.AddAuthorization();


        if (config.Development.EnableSwagger)
        {
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen(
                c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Neptune Rest Server", Version = "v1" }); }
            );
        }


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

        builder.Services.RegisterServiceToLoadAtStartup<IDatabaseService>();

        builder.Services.AddHostedService<NeptuneHostedService>();

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        // Configure the HTTP request pipeline.
        if (config.Development.EnableSwagger)
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neptune v1"));
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


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

        return (await File.ReadAllTextAsync(configFile)).FromYaml<NeptuneServerConfig>();
    }

    private static void InitJwtAuth(IServiceCollection services, NeptuneServerConfig config)
    {
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
                            new SymmetricSecurityKey(config.JwtAuth.Secret.FromBase64ToByteArray())
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
