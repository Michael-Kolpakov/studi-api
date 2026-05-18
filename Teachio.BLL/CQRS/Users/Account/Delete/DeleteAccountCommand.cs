using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Account.Request.Delete;
using Teachio.BLL.DTOs.Users.Account.Response;

namespace Teachio.BLL.CQRS.Users.Account.Delete;

public record DeleteAccountCommand(AccountDeleteRequestDto AccountDeleteRequestDto)
    : IRequest<Result<AppUserResponseDto>>;
