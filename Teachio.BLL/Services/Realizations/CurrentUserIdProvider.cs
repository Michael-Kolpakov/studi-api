using System.Security.Claims;

namespace Teachio.BLL.Services.Realizations;

public static class CurrentUserIdProvider
{
    public static Guid GetUserId(ClaimsPrincipal? user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // if (string.IsNullOrEmpty(userIdClaim))
        // {
        //     throw new InvalidOperationException("User ID not found in claims");
        // }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        // throw new UnauthorizedAccessException("User is not authenticated");

        return Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");
    }
}
