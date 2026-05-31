using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Response;

namespace Studi.BLL.CQRS.Courses.Videos.VideoProgress.Update;

public record UpdateVideoProgressCommand(VideoProgressUpdateRequestDto VideoProgressUpdateRequestDto)
    : IRequest<Result<VideoProgressResponseDto>>;
