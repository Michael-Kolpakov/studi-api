using Moq;
using Studi.BLL.Services.Interfaces;

namespace Studi.XUnitTests.Mocks;

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
