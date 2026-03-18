using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Create;

/// <summary>
/// Represents the <see cref="CreateVideoCommand"/> record model.
/// </summary>
/// <param name="VideoCreateRequestDto">The request payload in <paramref name="VideoCreateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record CreateVideoCommand(VideoCreateRequestDto VideoCreateRequestDto, Guid RequestingUserId)
    : IRequest<Result<VideoResponseDto>>;
