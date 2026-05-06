using FluentResults;
using Teachio.BLL.Models.Media;

namespace Teachio.BLL.Services.Interfaces;

public interface IVideoMetadataService
{
    Task<Result<VideoFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
