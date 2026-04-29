using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Upload;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;

public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto, Guid RequestingUserId)
    : IRequest<Result<ThumbnailUploadResponseDto>>;
