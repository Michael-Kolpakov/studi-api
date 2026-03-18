using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.VideoProgress.Update;

/// <summary>
/// Represents the <see cref="UpdateVideoProgressCommand"/> record model.
/// </summary>
/// <param name="VideoProgressUpdateRequestDto">The request payload in <paramref name="VideoProgressUpdateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record UpdateVideoProgressCommand(VideoProgressUpdateRequestDto VideoProgressUpdateRequestDto, Guid RequestingUserId)
    : IRequest<Result<VideoProgressResponseDto>>;
