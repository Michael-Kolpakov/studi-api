namespace Teachio.BLL.Services.Interfaces;

/// <summary>
/// Defines the contract for <see cref="ILoggerService"/>.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogInformation(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogWarning(string message);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogDebug(string message);

    /// <summary>
    /// Logs an error with optional request context and stack trace.
    /// </summary>
    /// <param name="request">The request context associated with the error, if available.</param>
    /// <param name="errorMessage">The error message to log.</param>
    /// <param name="stackTrace">The stack trace associated with the error, if available.</param>
    void LogError(object? request, string errorMessage, string? stackTrace = null);
}
