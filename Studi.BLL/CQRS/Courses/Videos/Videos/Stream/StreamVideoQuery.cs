using FluentResults;
using MediatR;
using Studi.BLL.Models.Storage;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.Stream;

public record StreamVideoQuery(Guid VideoId, string? RangeHeader)
    : IRequest<Result<GoogleDriveStreamResult>>;
