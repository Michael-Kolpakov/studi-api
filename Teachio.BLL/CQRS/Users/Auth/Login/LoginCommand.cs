using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Auth.Request;
using Teachio.BLL.DTOs.Users.Auth.Response;

namespace Teachio.BLL.CQRS.Users.Auth.Login;

public record LoginCommand(AuthLoginRequestDto AuthLoginRequestDto)
    : IRequest<Result<AuthTokenPairDto>>;
