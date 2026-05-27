using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Account.Request.Update;
using Studi.BLL.DTOs.Users.Account.Response;

namespace Studi.BLL.CQRS.Users.Account.Update;

public record UpdateAccountCommand(AccountUpdateRequestDto AccountUpdateRequestDto)
    : IRequest<Result<AppUserResponseDto>>;
