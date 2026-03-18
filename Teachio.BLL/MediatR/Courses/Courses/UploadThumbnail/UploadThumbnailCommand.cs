using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;

/// <summary>
/// Represents the <see cref="UploadThumbnailCommand"/> record model.
/// </summary>
/// <param name="ThumbnailUploadRequestDto">The request payload in <paramref name="ThumbnailUploadRequestDto"/>.</param>
public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto)
    : IRequest<Result<ThumbnailUploadResponseDto>>;
