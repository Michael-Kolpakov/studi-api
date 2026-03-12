using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Delete;

public record DeleteVideoCommand(Guid videoId, Guid requestingUserId)
    : IRequest<Result<VideoResponseDto>>;
