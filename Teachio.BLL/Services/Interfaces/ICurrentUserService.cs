namespace Teachio.BLL.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();

    bool TryGetUserId(out Guid userId);
}
