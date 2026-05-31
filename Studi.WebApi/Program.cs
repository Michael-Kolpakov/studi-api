using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Serilog;
using Studi.WebApi.Extensions;
using Studi.WebApi.Middlewares;
using ILogger = Serilog.ILogger;

namespace Studi.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureApplication(builder);
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        var cancellationToken = lifetime.ApplicationStopping;
        var environment = app.Services.GetRequiredService<IHostEnvironment>();
        var startupLogger = app.Services.GetRequiredService<ILogger>();

        app.UseRequestLocalization(new RequestLocalizationOptions()
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedCultures =
            [
                new CultureInfo("en")
            ]
        });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCustomSwagger();
        app.MapControllers();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (!string.Equals(environment.EnvironmentName, "IntegrationTests", StringComparison.OrdinalIgnoreCase))
        {
            await DatabaseExtensions.InitializeDatabase(app, cancellationToken);
        }

        startupLogger.Information("Application started and ready to accept requests.");

        await app.RunAsync();
    }
}
