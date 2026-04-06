using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;

public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto)
    : IRequest<Result<ThumbnailUploadResponseDto>>;
