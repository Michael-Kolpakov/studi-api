using Serilog;
using Studi.BLL.Services.Interfaces;

namespace Studi.BLL.Services.Realizations;

public class LoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogInformation(string message)
    {
        _logger.Information("{Message}", message);
    }

    public void LogWarning(string message)
    {
        _logger.Warning("{Message}", message);
    }

    public void LogDebug(string message)
    {
        _logger.Debug("{Message}", message);
    }

    public void LogError(object? request, string errorMessage, string? stackTrace = null)
    {
        var stackTraceInfo = stackTrace is not null
            ? $"\nStackTrace: {stackTrace}"
            : string.Empty;

        if (request is not null)
        {
            var requestType = request.GetType().ToString();
            var requestClass = requestType[(requestType.LastIndexOf('.') + 1)..];

            _logger.Error(
                "Request '{Request}' handled with the error: {ErrorMessage}{StackTraceInfo}",
                requestClass,
                errorMessage,
                stackTraceInfo);
        }
        else
        {
            _logger.Error(
                "Error occurred: {ErrorMessage}{StackTraceInfo}",
                errorMessage,
                stackTraceInfo);
        }
    }
}
