using Moq;
using Studi.BLL.Services.Interfaces;

namespace Studi.XUnitTests.Verifications;

public static class LoggerVerifications
{
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
