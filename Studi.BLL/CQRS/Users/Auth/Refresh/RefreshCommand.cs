using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Auth.Response;

namespace Studi.BLL.CQRS.Users.Auth.Refresh;

public record RefreshCommand(string? RefreshToken)
    : IRequest<Result<AuthTokenPairDto>>;
