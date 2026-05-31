using FluentResults;
using MediatR;

namespace Studi.BLL.CQRS.Users.Auth.Logout;

public record LogoutCommand(string? RefreshToken)
    : IRequest<Result>;
