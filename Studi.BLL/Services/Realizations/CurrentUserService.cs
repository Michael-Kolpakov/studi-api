using Microsoft.AspNetCore.Http;
using Studi.BLL.Services.Interfaces;

namespace Studi.BLL.Services.Realizations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
        => CurrentUserIdProvider.GetUserId(_httpContextAccessor.HttpContext?.User);

    public bool TryGetUserId(out Guid userId)
        => CurrentUserIdProvider.TryGetUserId(_httpContextAccessor.HttpContext?.User, out userId);
}
