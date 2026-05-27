using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Auth.Request;
using Studi.BLL.DTOs.Users.Auth.Response;

namespace Studi.BLL.CQRS.Users.Auth.Register;

public record RegisterCommand(AuthRegisterRequestDto AuthRegisterRequestDto)
    : IRequest<Result<RegistrationPinResponseDto>>;
