using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.GetBySectionId;

public record GetVideosBySectionIdQuery(Guid SectionId)
    : IRequest<Result<SectionVideosResponseDto>>;
