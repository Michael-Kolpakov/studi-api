using FluentResults;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.Services.Interfaces;

public interface IAuthTokenIssuer
{
    Task<Result<AuthTokenPairDto>> IssueTokenPairAsync(AppUser user, CancellationToken cancellationToken);
}
