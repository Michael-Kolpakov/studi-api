using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Create;

public record CreateVideoCommand(VideoCreateRequestDto VideoCreateRequestDto)
    : IRequest<Result<VideoResponseDto>>;
