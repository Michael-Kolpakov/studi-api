using Teachio.BLL.Models.Auth;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.Services.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(AppUser user);
}
