using System.Reflection;
using Serilog;
using Serilog.Events;

namespace Teachio.WebApi.Extensions;

public static class SerilogExtension
{
    private const string _consoleLogTemplate = "[{Timestamp:HH:mm:ss.fff} [{Level}] {SourceContext} {Message}{NewLine}{Exception}";

    public static void AddSerilogLogging(this IServiceCollection _)
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
            .WriteTo.Console(LogEventLevel.Information, _consoleLogTemplate);

        Log.Logger = loggerConfiguration.CreateLogger();
    }
}
