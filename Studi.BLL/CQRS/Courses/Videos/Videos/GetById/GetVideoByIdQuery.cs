using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.GetById;

public record GetVideoByIdQuery(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
