using FluentResults;
using Studi.BLL.DTOs.Users.Auth.Response;
using Studi.DAL.Entities.Users.Users;

namespace Studi.BLL.Services.Interfaces;

public interface IAuthTokenIssuer
{
    Task<Result<AuthTokenPairDto>> IssueTokenPairAsync(AppUser user, CancellationToken cancellationToken);
}
