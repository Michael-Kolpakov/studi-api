using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Account.Request.Delete;
using Studi.BLL.DTOs.Users.Account.Response;

namespace Studi.BLL.CQRS.Users.Account.Delete;

public record DeleteAccountCommand(AccountDeleteRequestDto AccountDeleteRequestDto)
    : IRequest<Result<AppUserResponseDto>>;
