using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Update;

public record UpdateVideoCommand(VideoUpdateRequestDto videoUpdateRequestDto, Guid requestingUserId)
    : IRequest<Result<VideoResponseDto>>;
