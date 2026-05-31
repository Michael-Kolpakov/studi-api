using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Users.Account.Response;

namespace Studi.BLL.CQRS.Users.Account.GetCurrent;

public record GetCurrentUserQuery : IRequest<Result<AppUserShortResponseDto>>;
