using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Services.Realizations;

namespace Teachio.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId() => CurrentUserIdProvider.GetUserId(_httpContextAccessor.HttpContext?.User);
}
