using Serilog;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.BLL.Services.Realizations;

public class LoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    public void LogError(object? request, string errorMessage, string? stackTrace = null)
    {
        if (request is not null)
        {
            var requestType = request.GetType().ToString();
            var requestClass = requestType.Substring(requestType.LastIndexOf('.') + 1);
            var stackTraceInfo = stackTrace is not null ? $"\nStackTrace: {stackTrace}" : string.Empty;

            _logger.Error($"'{requestClass}' handled with the error: {errorMessage}{stackTraceInfo}");
        }
        else
        {
            _logger.Error(errorMessage);
        }
    }
}
