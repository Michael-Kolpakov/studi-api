using FluentResults;
using MediatR;
using Teachio.BLL.Models.Storage;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Stream;

public record StreamVideoQuery(Guid VideoId, string? RangeHeader)
    : IRequest<Result<GoogleDriveStreamResult>>;
