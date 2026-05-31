using Studi.BLL.Models.Auth;
using Studi.DAL.Entities.Users.Users;

namespace Studi.BLL.Services.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(AppUser user);
}
