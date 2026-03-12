using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Create;

public record CreateVideoCommand(VideoCreateRequestDto videoCreateRequestDto, Guid requestingUserId)
    : IRequest<Result<VideoResponseDto>>;
