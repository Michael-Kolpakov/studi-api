using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Create;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.Create;

public record CreateVideoCommand(VideoCreateRequestDto VideoCreateRequestDto)
    : IRequest<Result<VideoResponseDto>>;
