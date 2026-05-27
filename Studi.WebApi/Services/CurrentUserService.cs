using Studi.BLL.Services.Interfaces;
using Studi.BLL.Services.Realizations;

namespace Studi.WebApi.Services;

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
