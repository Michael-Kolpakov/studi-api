using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Delete;

public record DeleteVideoCommand(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
