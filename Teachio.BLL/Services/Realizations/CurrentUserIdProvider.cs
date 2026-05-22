using System.Security.Claims;

namespace Teachio.BLL.Services.Realizations;

public static class CurrentUserIdProvider
{
    public static Guid GetUserId(ClaimsPrincipal? user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new InvalidOperationException("User ID not found in claims");
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User is not authenticated");
    }

    public static bool TryGetUserId(ClaimsPrincipal? user, out Guid userId)
    {
        userId = Guid.Empty;

        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return false;
        }

        return Guid.TryParse(userIdClaim, out userId);
    }
}
