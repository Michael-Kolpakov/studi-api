using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Users.Account.Response;

namespace Teachio.BLL.CQRS.Users.Account.GetCurrent;

public record GetCurrentUserQuery : IRequest<Result<AppUserShortResponseDto>>;
