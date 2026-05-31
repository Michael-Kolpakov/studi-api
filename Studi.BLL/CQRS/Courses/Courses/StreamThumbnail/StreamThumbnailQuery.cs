using FluentResults;
using MediatR;
using Studi.BLL.Models.Storage;

namespace Studi.BLL.CQRS.Courses.Courses.StreamThumbnail;

public record StreamThumbnailQuery(Guid CourseId, string? RangeHeader)
    : IRequest<Result<GoogleDriveStreamResult>>;
