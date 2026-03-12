using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.VideoProgress.Update;

public record UpdateVideoProgressCommand(VideoProgressUpdateRequestDto videoProgressUpdateRequestDto, Guid requestingUserId)
    : IRequest<Result<VideoProgressResponseDto>>;
