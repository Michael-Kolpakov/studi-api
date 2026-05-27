using FluentResults;
using Studi.BLL.Models.Media;

namespace Studi.BLL.Services.Interfaces;

public interface IVideoMetadataService
{
    Task<Result<VideoFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
