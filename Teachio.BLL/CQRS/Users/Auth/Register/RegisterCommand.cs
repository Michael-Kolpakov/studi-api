using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Auth.Request;
using Teachio.BLL.DTOs.Users.Auth.Response;

namespace Teachio.BLL.CQRS.Users.Auth.Register;

public record RegisterCommand(AuthRegisterRequestDto AuthRegisterRequestDto)
    : IRequest<Result<AuthTokenPairDto>>;
