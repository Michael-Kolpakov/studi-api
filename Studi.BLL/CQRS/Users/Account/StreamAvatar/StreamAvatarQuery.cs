using FluentResults;
using MediatR;
using Studi.BLL.Models.Storage;

namespace Studi.BLL.CQRS.Users.Account.StreamAvatar;

public record StreamAvatarQuery(string? RangeHeader, Guid? CourseId = null)
    : IRequest<Result<GoogleDriveStreamResult>>;
