using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.UpdateOrderIndex;

public record UpdateVideoOrderIndexCommand(VideoUpdateOrderIndexRequestDto VideoUpdateOrderIndexRequestDto)
    : IRequest<Result<VideoResponseDto>>;
