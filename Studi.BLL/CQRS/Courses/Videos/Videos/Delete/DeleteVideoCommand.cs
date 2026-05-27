using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.Delete;

public record DeleteVideoCommand(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
