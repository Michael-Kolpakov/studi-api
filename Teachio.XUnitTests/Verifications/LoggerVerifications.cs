using Moq;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.XUnitTests.Verifications;

/// <summary>
/// Represents the <see cref="LoggerVerifications"/> type.
/// </summary>
public static class LoggerVerifications
{
    /// <summary>
    /// Verifies that an error log entry with the expected payload was written exactly once.
    /// </summary>
    /// <param name="mockLoggerService">The logger service mock under verification.</param>
    /// <param name="request">The request object expected in the log call.</param>
    /// <param name="expectedMessage">The expected error message.</param>
    public static void VerifyLoggerErrorWasCalled(
        Mock<ILoggerService> mockLoggerService,
        object request,
        string expectedMessage)
    {
        mockLoggerService.Verify(
            logger => logger.LogError(request, expectedMessage, null),
            Times.Once);
    }
}
