using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Account.Request.Update;
using Teachio.BLL.DTOs.Users.Account.Response;

namespace Teachio.BLL.CQRS.Users.Account.Update;

public record UpdateAccountCommand(AccountUpdateRequestDto AccountUpdateRequestDto)
    : IRequest<Result<AppUserResponseDto>>;
