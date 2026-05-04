using FluentResults;
using Teachio.BLL.Models.Storage;

namespace Teachio.BLL.Services.Interfaces;

public interface IGoogleDriveStorageService
{
    Task<Result<GoogleDriveUploadResult>> UploadFileAsync(
        IEnumerable<string> folderSegments,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

    Task<Result> DeleteFileByPathAsync(
        IEnumerable<string> folderSegments,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFolderByPathAsync(
        IEnumerable<string> folderSegments,
        CancellationToken cancellationToken = default);
}
