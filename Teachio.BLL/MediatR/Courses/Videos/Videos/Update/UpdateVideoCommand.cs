using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Update;

/// <summary>
/// Represents the <see cref="UpdateVideoCommand"/> record model.
/// </summary>
/// <param name="VideoUpdateRequestDto">The request payload in <paramref name="VideoUpdateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record UpdateVideoCommand(VideoUpdateRequestDto VideoUpdateRequestDto, Guid RequestingUserId)
    : IRequest<Result<VideoResponseDto>>;
