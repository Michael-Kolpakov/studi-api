using FluentResults;
using Teachio.BLL.Models.Media;

namespace Teachio.BLL.Services.Interfaces;

public interface IAvatarMetadataService
{
    Task<Result<AvatarFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
