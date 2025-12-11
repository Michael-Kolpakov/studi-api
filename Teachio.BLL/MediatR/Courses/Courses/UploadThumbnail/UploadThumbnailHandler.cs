using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Utils.MappingResolvers;

namespace Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;

public class UploadThumbnailHandler : IRequestHandler<UploadThumbnailCommand, Result<ThumbnailUploadResponseDto>>
{
    // TODO: move validation logic to the separate DTO model Attribute
    private static readonly string[] AllowedContentTypes =
    [
        "jpg",
        "jpeg",
        "png",
        "webp"
    ];

    private readonly ILoggerService _logger;

    public UploadThumbnailHandler(ILoggerService logger)
    {
        _logger = logger;
    }

    public async Task<Result<ThumbnailUploadResponseDto>> Handle(UploadThumbnailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to upload a thumbnail of a course");

        var fileName = NameFromTitleResolver.CreateNameFromTitle(request.ThumbnailUploadRequestDto.ThumbnailFile.FileName);
        var fileContentType = request.ThumbnailUploadRequestDto.ThumbnailFile.ContentType;

        if (!AllowedContentTypes.Contains(fileContentType))
        {
            var errorMessage = $"Unsupported thumbnail content type. Allowed types are: {string.Join(", ", AllowedContentTypes)}";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var thumbnailName = string.Join(".", fileName, fileContentType);

        // TODO: address Google Drive API (or CDN in the future) to upload thumbnail image

        // TODO: save thumbnail metadata to the database

        var thumbnailUploadResponseDto = new ThumbnailUploadResponseDto()
        {
            ThumbnailName = thumbnailName
        };

        return Result.Ok(thumbnailUploadResponseDto);
    }
}
