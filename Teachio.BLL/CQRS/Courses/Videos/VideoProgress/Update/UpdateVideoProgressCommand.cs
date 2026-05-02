using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.VideoProgress.Update;

public record UpdateVideoProgressCommand(VideoProgressUpdateRequestDto VideoProgressUpdateRequestDto)
    : IRequest<Result<VideoProgressResponseDto>>;
