using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.GetBySectionId;

public record GetVideosBySectionIdQuery(Guid SectionId)
    : IRequest<Result<SectionVideosResponseDto>>;
