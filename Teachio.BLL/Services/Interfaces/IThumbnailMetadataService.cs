using FluentResults;
using Teachio.BLL.Models.Media;

namespace Teachio.BLL.Services.Interfaces;

public interface IThumbnailMetadataService
{
    Task<Result<ThumbnailFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
