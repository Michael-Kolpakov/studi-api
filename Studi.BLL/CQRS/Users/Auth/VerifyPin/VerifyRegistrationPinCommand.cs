using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Auth.Request;
using Studi.BLL.DTOs.Users.Auth.Response;

namespace Studi.BLL.CQRS.Users.Auth.VerifyPin;

public record VerifyRegistrationPinCommand(VerifyRegistrationPinRequestDto VerifyRegistrationPinRequestDto)
    : IRequest<Result<AuthTokenPairDto>>;
