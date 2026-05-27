using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Auth.Request;
using Studi.BLL.DTOs.Users.Auth.Response;

namespace Studi.BLL.CQRS.Users.Auth.Login;

public record LoginCommand(AuthLoginRequestDto AuthLoginRequestDto)
    : IRequest<Result<AuthTokenPairDto>>;
