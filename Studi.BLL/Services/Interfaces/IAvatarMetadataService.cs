using FluentResults;
using Studi.BLL.Models.Media;

namespace Studi.BLL.Services.Interfaces;

public interface IAvatarMetadataService
{
    Task<Result<AvatarFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default);
}
