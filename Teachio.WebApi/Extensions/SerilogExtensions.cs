using System.Globalization;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace Teachio.WebApi.Extensions;

/// <summary>
/// Represents the <see cref="SerilogExtensions"/> type.
/// </summary>
public static class SerilogExtensions
{
    private const string ConsoleLogTemplate = "[{Timestamp:HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="services">The <paramref name="services"/> argument.</param>
    public static void AddSerilogLogging(this IServiceCollection services)
    {
        var projectName = Assembly.GetCallingAssembly().GetName().Name?.ToLowerInvariant();

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("ProjectName", projectName)
            .WriteTo.Console(LogEventLevel.Information, ConsoleLogTemplate, CultureInfo.InvariantCulture);

        Log.Logger = loggerConfiguration.CreateLogger();

        services.AddSingleton(Log.Logger);
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(Log.Logger, dispose: false);
        });
    }
}
