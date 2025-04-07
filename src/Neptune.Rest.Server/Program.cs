using Microsoft.OpenApi.Models;
using Serilog;

namespace Neptune.Rest.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();


        builder.Logging.ClearProviders().AddSerilog();
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();


        builder.Services.AddSwaggerGen(
            c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Neptune Rest Server", Version = "v1" }); }
        );

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neptune v1"));
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.Run();
    }
}
