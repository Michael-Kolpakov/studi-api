using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.GetById;

public record GetVideoByIdQuery(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
