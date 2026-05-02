using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Update;

public record UpdateVideoCommand(VideoUpdateRequestDto VideoUpdateRequestDto)
    : IRequest<Result<VideoResponseDto>>;
