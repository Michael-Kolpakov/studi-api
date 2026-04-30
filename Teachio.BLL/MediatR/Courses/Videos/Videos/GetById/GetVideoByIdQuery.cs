using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.GetById;

public record GetVideoByIdQuery(Guid VideoId)
    : IRequest<Result<VideoResponseDto>>;
