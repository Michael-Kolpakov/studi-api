using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Delete;

/// <summary>
/// Represents the <see cref="DeleteVideoCommand"/> record model.
/// </summary>
/// <param name="VideoId">The identifier of <paramref name="VideoId"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record DeleteVideoCommand(Guid VideoId, Guid RequestingUserId)
    : IRequest<Result<VideoResponseDto>>;
