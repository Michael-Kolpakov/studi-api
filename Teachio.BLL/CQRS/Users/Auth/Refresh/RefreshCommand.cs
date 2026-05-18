using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Auth.Response;

namespace Teachio.BLL.CQRS.Users.Auth.Refresh;

public record RefreshCommand(string? RefreshToken)
    : IRequest<Result<AuthTokenPairDto>>;
