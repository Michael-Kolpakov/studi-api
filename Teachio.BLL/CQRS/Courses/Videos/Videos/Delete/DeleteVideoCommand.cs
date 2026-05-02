using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Delete;

public record DeleteVideoCommand(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
