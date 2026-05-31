using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.Update;

public record UpdateVideoCommand(VideoUpdateRequestDto VideoUpdateRequestDto)
    : IRequest<Result<VideoResponseDto>>;
