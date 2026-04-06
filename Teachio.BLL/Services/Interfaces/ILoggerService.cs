namespace Teachio.BLL.Services.Interfaces;

public interface ILoggerService
{
    void LogInformation(string message);

    void LogWarning(string message);

    void LogDebug(string message);

    void LogError(object? request, string errorMessage, string? stackTrace = null);
}
