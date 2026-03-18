using Serilog;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.BLL.Services.Realizations;

/// <summary>
/// Represents the <see cref="LoggerService"/> type.
/// </summary>
public class LoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Performs the LogInformation operation.
    /// </summary>
    /// <param name="message">The <paramref name="message"/> argument.</param>
    public void LogInformation(string message)
    {
        _logger.Information("{Message}", message);
    }

    /// <summary>
    /// Performs the LogWarning operation.
    /// </summary>
    /// <param name="message">The <paramref name="message"/> argument.</param>
    public void LogWarning(string message)
    {
        _logger.Warning("{Message}", message);
    }

    /// <summary>
    /// Performs the LogDebug operation.
    /// </summary>
    /// <param name="message">The <paramref name="message"/> argument.</param>
    public void LogDebug(string message)
    {
        _logger.Debug("{Message}", message);
    }

    /// <summary>
    /// Performs the LogError operation.
    /// </summary>
    /// <param name="request">The request payload in <paramref name="request"/>.</param>
    /// <param name="errorMessage">The <paramref name="errorMessage"/> argument.</param>
    /// <param name="stackTrace">The <paramref name="stackTrace"/> argument.</param>
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
