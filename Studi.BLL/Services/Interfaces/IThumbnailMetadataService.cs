using FluentResults;
using Studi.BLL.Models.Media;

namespace Studi.BLL.Services.Interfaces;

public interface IThumbnailMetadataService
{
    Task<Result<ThumbnailFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
