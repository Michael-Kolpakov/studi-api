using FluentResults;
using MediatR;

namespace Teachio.BLL.CQRS.Users.Auth.Logout;

public record LogoutCommand(string? RefreshToken)
    : IRequest<Result>;
