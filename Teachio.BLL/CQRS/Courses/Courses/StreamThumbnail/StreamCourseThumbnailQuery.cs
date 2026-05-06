using FluentResults;
using MediatR;
using Teachio.BLL.Models.Storage;

namespace Teachio.BLL.CQRS.Courses.Courses.StreamThumbnail;

public record StreamCourseThumbnailQuery(Guid CourseId, string? RangeHeader)
    : IRequest<Result<GoogleDriveStreamResult>>;
