using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Utils.MappingResolvers;

namespace Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;

public class UploadThumbnailHandler : IRequestHandler<UploadThumbnailCommand, Result<ThumbnailUploadResponseDto>>
{
    // TODO: move validation logic to the separate DTO model Attribute
    private static readonly string[] _allowedContentTypes =
    [
        "jpg",
        "jpeg",
        "png"
    ];

    private readonly ILoggerService _logger;

    public UploadThumbnailHandler(ILoggerService logger)
    {
        _logger = logger;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<Result<ThumbnailUploadResponseDto>> Handle(UploadThumbnailCommand request, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to upload a thumbnail of a course");

        var fileName = NameFromTitleResolver.CreateNameFromTitle(request.ThumbnailUploadRequestDto.ThumbnailFile.FileName);
        var fileContentType = request.ThumbnailUploadRequestDto.ThumbnailFile.ContentType;

        if (!_allowedContentTypes.Contains(fileContentType))
        {
            var errorMessage = $"Unsupported thumbnail content type. Allowed types are: {string.Join(", ", _allowedContentTypes)}";
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
