using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Request.Upload;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.UploadThumbnail;

public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto)
    : IRequest<Result<ThumbnailUploadResponseDto>>;
