using Moq;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.XUnitTests.Mocks;

public static class CurrentUserMocks
{
    public static void SetupGetUserIdMock(
        Mock<ICurrentUserService> mockCurrentUserService,
        Guid userId)
    {
        mockCurrentUserService
            .Setup(service => service.GetUserId())
            .Returns(userId);
    }
}
