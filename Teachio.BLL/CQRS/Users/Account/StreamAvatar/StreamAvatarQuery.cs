using FluentResults;
using MediatR;
using Teachio.BLL.Models.Storage;

namespace Teachio.BLL.CQRS.Users.Account.StreamAvatar;

public record StreamAvatarQuery(string? RangeHeader, Guid? CourseId = null)
    : IRequest<Result<GoogleDriveStreamResult>>;
