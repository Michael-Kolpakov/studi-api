using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Request.Upload;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.UploadThumbnail;

public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto)
    : IRequest<Result<ThumbnailUploadResponseDto>>;
