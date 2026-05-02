using FluentResults;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Teachio.BLL.Models.Storage;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResources;
using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace Teachio.BLL.Services.Realizations;

public class GoogleDriveStorageService : IGoogleDriveStorageService
{
    private const string FolderMimeType = "application/vnd.google-apps.folder";

    private readonly GoogleDriveStorageOptions _options;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<GoogleDriveStorageSharedResource> _stringLocalizerGoogleDriveStorage;

    public GoogleDriveStorageService(
        IOptions<GoogleDriveStorageOptions> options,
        ILoggerService logger,
        IStringLocalizer<GoogleDriveStorageSharedResource> stringLocalizerGoogleDriveStorage)
    {
        _options = options.Value;
        _logger = logger;
        _stringLocalizerGoogleDriveStorage = stringLocalizerGoogleDriveStorage;
    }

    public async Task<Result<GoogleDriveUploadResult>> UploadFileAsync(
        IEnumerable<string> folderSegments,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.FileNameIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.ContentTypeIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (fileStream is null)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.FileStreamIsNull)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (!fileStream.CanRead)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.FileStreamNotReadable)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var folderSegmentsList = folderSegments
            .Where(segment => !string.IsNullOrWhiteSpace(segment))
            .Select(segment => segment.Trim())
            .ToList();

        var driveServiceResult = TryCreateDriveService();

        if (driveServiceResult.IsFailed)
        {
            return Result.Fail(driveServiceResult.Errors[0].Message);
        }

        try
        {
            using var driveService = driveServiceResult.Value;
            var currentFolderId = await GetRootFolderIdAsync(
                driveService,
                createIfMissing: true,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(currentFolderId))
            {
                var errorMessage = _stringLocalizerGoogleDriveStorage[
                    nameof(GoogleDriveStorageSharedResource_en.RootFolderIsNotAvailable)
                ].Value;

                return Result.Fail(errorMessage);
            }

            foreach (var folderName in folderSegmentsList)
            {
                currentFolderId = await EnsureFolderAsync(driveService, currentFolderId, folderName, cancellationToken);
            }

            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            var fileMetadata = new DriveFile
            {
                Name = fileName,
                Parents = [currentFolderId]
            };

            var uploadRequest = driveService.Files.Create(fileMetadata, fileStream, contentType);
            uploadRequest.Fields = "id,webViewLink";
            uploadRequest.SupportsAllDrives = true;

            var uploadStatus = await uploadRequest.UploadAsync(cancellationToken);

            if (uploadStatus.Status != UploadStatus.Completed)
            {
                var uploadErrorMessage = uploadStatus.Exception?.Message
                                         ?? _stringLocalizerGoogleDriveStorage[
                                             nameof(GoogleDriveStorageSharedResource_en.UploadFailedWithStatus),
                                             uploadStatus.Status
                                         ].Value;

                _logger.LogError(null, uploadErrorMessage, uploadStatus.Exception?.ToString());

                return Result.Fail(uploadErrorMessage);
            }

            var uploadedFile = uploadRequest.ResponseBody;

            if (uploadedFile?.Id is null)
            {
                var errorMessage = _stringLocalizerGoogleDriveStorage[
                    nameof(GoogleDriveStorageSharedResource_en.UploadedFileIdentifierNotReturned)
                ].Value;
                _logger.LogError(null, errorMessage);

                return Result.Fail(errorMessage);
            }

            return Result.Ok(new GoogleDriveUploadResult
            {
                FileId = uploadedFile.Id,
                WebViewLink = uploadedFile.WebViewLink
            });
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.UploadFailed)
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    public async Task<Result> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.FileIdentifierIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var driveServiceResult = TryCreateDriveService();

        if (driveServiceResult.IsFailed)
        {
            return Result.Fail(driveServiceResult.Errors[0].Message);
        }

        try
        {
            using var driveService = driveServiceResult.Value;

            return await DeleteFileByIdInternalAsync(driveService, fileId, cancellationToken);
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.DeleteFileFailed),
                fileId
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    public async Task<Result> DeleteFileByPathAsync(
        IEnumerable<string> folderSegments,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.FileNameIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var folderSegmentsList = folderSegments
            .Where(segment => !string.IsNullOrWhiteSpace(segment))
            .Select(segment => segment.Trim())
            .ToList();

        var driveServiceResult = TryCreateDriveService();

        if (driveServiceResult.IsFailed)
        {
            return Result.Fail(driveServiceResult.Errors[0].Message);
        }

        try
        {
            using var driveService = driveServiceResult.Value;

            var currentFolderId = await GetRootFolderIdAsync(
                driveService,
                createIfMissing: false,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(currentFolderId))
            {
                return Result.Ok();
            }

            foreach (var folderName in folderSegmentsList)
            {
                currentFolderId = await FindFolderIdAsync(
                    driveService,
                    currentFolderId,
                    folderName,
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(currentFolderId))
                {
                    return Result.Ok();
                }
            }

            var fileIdResult = await FindFileIdByNameAsync(
                driveService,
                currentFolderId,
                fileName,
                cancellationToken);

            if (fileIdResult.IsFailed)
            {
                return Result.Fail(fileIdResult.Errors[0].Message);
            }

            if (string.IsNullOrWhiteSpace(fileIdResult.Value))
            {
                return Result.Ok();
            }

            return await DeleteFileByIdInternalAsync(driveService, fileIdResult.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.DeleteFileByPathFailed),
                fileName
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    private async Task<Result<string?>> FindFileIdByNameAsync(
        DriveService driveService,
        string parentFolderId,
        string fileName,
        CancellationToken cancellationToken)
    {
        var escapedFileName = EscapeForDriveQuery(fileName);
        var escapedParentFolderId = EscapeForDriveQuery(parentFolderId);

        var listRequest = driveService.Files.List();
        listRequest.Q = $"name = '{escapedFileName}' and trashed = false and '{escapedParentFolderId}' in parents";
        listRequest.Fields = "files(id,name)";
        listRequest.PageSize = 2;
        listRequest.Spaces = "drive";
        listRequest.SupportsAllDrives = true;
        listRequest.IncludeItemsFromAllDrives = true;

        var files = await listRequest.ExecuteAsync(cancellationToken);

        if (files.Files.Count == 0)
        {
            return Result.Ok<string?>(null);
        }

        if (files.Files.Count > 1)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.MultipleFilesWithSameName),
                fileName
            ].Value;

            return Result.Fail(errorMessage);
        }

        return Result.Ok<string?>(files.Files[0].Id);
    }

    private static async Task<Result> DeleteFileByIdInternalAsync(
        DriveService driveService,
        string fileId,
        CancellationToken cancellationToken)
    {
            var deleteRequest = driveService.Files.Delete(fileId);
            deleteRequest.SupportsAllDrives = true;

            await deleteRequest.ExecuteAsync(cancellationToken);

            return Result.Ok();
    }

    private Result<DriveService> TryCreateDriveService()
    {
        if (string.IsNullOrWhiteSpace(_options.RootFolderName) && string.IsNullOrWhiteSpace(_options.RootFolderId))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.RootFolderNotConfigured)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var credentialInitializerResult = TryCreateCredentialInitializer();

        if (credentialInitializerResult.IsFailed)
        {
            return Result.Fail(credentialInitializerResult.Errors[0].Message);
        }

        try
        {
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentialInitializerResult.Value,
                ApplicationName = _options.ApplicationName
            });

            return Result.Ok(driveService);
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.InitializeClientFailed)
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    private Result<IConfigurableHttpClientInitializer> TryCreateCredentialInitializer()
    {
        var hasServiceAccountCredentialPath = !string.IsNullOrWhiteSpace(_options.ServiceAccountCredentialPath);
        var hasServiceAccountCredentialJson = !string.IsNullOrWhiteSpace(_options.ServiceAccountCredentialJson);
        var hasAnyServiceAccountCredential = hasServiceAccountCredentialPath || hasServiceAccountCredentialJson;

        var hasOAuthClientId = !string.IsNullOrWhiteSpace(_options.OAuthClientId);
        var hasOAuthClientSecret = !string.IsNullOrWhiteSpace(_options.OAuthClientSecret);
        var hasOAuthRefreshToken = !string.IsNullOrWhiteSpace(_options.OAuthRefreshToken);

        var hasAnyOAuthCredential = hasOAuthClientId || hasOAuthClientSecret || hasOAuthRefreshToken;
        var hasCompleteOAuthCredential = hasOAuthClientId && hasOAuthClientSecret && hasOAuthRefreshToken;

        if (hasAnyOAuthCredential && !hasCompleteOAuthCredential)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.OAuthCredentialsIncomplete)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (hasServiceAccountCredentialPath && hasServiceAccountCredentialJson)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.ServiceAccountCredentialsAmbiguous)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (hasCompleteOAuthCredential && hasAnyServiceAccountCredential)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.CredentialsAmbiguous)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (!hasCompleteOAuthCredential && !hasAnyServiceAccountCredential)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.CredentialsNotConfigured)
            ].Value;

            return Result.Fail(errorMessage);
        }

        try
        {
            if (hasCompleteOAuthCredential)
            {
                var authorizationFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _options.OAuthClientId,
                        ClientSecret = _options.OAuthClientSecret
                    },
                    Scopes = [DriveService.ScopeConstants.Drive]
                });

                var tokenResponse = new TokenResponse
                {
                    RefreshToken = _options.OAuthRefreshToken
                };

                var oauthUserId = string.IsNullOrWhiteSpace(_options.OAuthUserId)
                    ? GoogleDriveStorageOptions.DefaultOAuthUserId
                    : _options.OAuthUserId;

                var userCredential = new UserCredential(authorizationFlow, oauthUserId, tokenResponse);

                return Result.Ok<IConfigurableHttpClientInitializer>(userCredential);
            }

            GoogleCredential serviceAccountCredential;

            if (hasServiceAccountCredentialPath)
            {
                var serviceAccountCredentialPath = _options.ServiceAccountCredentialPath!;

                if (!File.Exists(serviceAccountCredentialPath))
                {
                    var errorMessage = _stringLocalizerGoogleDriveStorage[
                        nameof(GoogleDriveStorageSharedResource_en.ServiceAccountCredentialFileNotFound),
                        serviceAccountCredentialPath
                    ].Value;

                    return Result.Fail(errorMessage);
                }

                serviceAccountCredential = CredentialFactory.FromFile(
                    serviceAccountCredentialPath,
                    JsonCredentialParameters.ServiceAccountCredentialType);
            }
            else
            {
                serviceAccountCredential = CredentialFactory.FromJson(
                    _options.ServiceAccountCredentialJson,
                    JsonCredentialParameters.ServiceAccountCredentialType);
            }

            var scopedServiceAccountCredential = serviceAccountCredential.CreateScoped(DriveService.ScopeConstants.Drive);

            return Result.Ok<IConfigurableHttpClientInitializer>(scopedServiceAccountCredential);
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.InitializeCredentialsFailed)
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    private async Task<string?> GetRootFolderIdAsync(
        DriveService driveService,
        bool createIfMissing,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.RootFolderId))
        {
            return _options.RootFolderId;
        }

        return createIfMissing
            ? await EnsureFolderAsync(driveService, null, _options.RootFolderName, cancellationToken)
            : await FindFolderIdAsync(driveService, null, _options.RootFolderName, cancellationToken);
    }

    private static string EscapeForDriveQuery(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\\'", StringComparison.Ordinal);
    }

    private static string BuildFolderQuery(string folderName, string? parentFolderId)
    {
        var escapedFolderName = EscapeForDriveQuery(folderName);
        var queryParts = new List<string>
        {
            $"name = '{escapedFolderName}'",
            $"mimeType = '{FolderMimeType}'",
            "trashed = false"
        };

        if (!string.IsNullOrWhiteSpace(parentFolderId))
        {
            var escapedParentFolderId = EscapeForDriveQuery(parentFolderId);
            queryParts.Add($"'{escapedParentFolderId}' in parents");
        }

        return string.Join(" and ", queryParts);
    }

    private async Task<string> EnsureFolderAsync(
        DriveService driveService,
        string? parentFolderId,
        string folderName,
        CancellationToken cancellationToken)
    {
        var listRequest = driveService.Files.List();
        listRequest.Q = BuildFolderQuery(folderName, parentFolderId);
        listRequest.Fields = "files(id, name)";
        listRequest.PageSize = 1;
        listRequest.Spaces = "drive";
        listRequest.SupportsAllDrives = true;
        listRequest.IncludeItemsFromAllDrives = true;

        var existingFolders = await listRequest.ExecuteAsync(cancellationToken);
        var existingFolderId = existingFolders.Files is { Count: > 0 }
            ? existingFolders.Files[0].Id
            : null;

        if (!string.IsNullOrWhiteSpace(existingFolderId))
        {
            return existingFolderId;
        }

        var folderMetadata = new DriveFile
        {
            Name = folderName,
            MimeType = FolderMimeType,
            Parents = string.IsNullOrWhiteSpace(parentFolderId)
                ? null
                : [parentFolderId]
        };

        var createRequest = driveService.Files.Create(folderMetadata);
        createRequest.Fields = "id";
        createRequest.SupportsAllDrives = true;

        var createdFolder = await createRequest.ExecuteAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(createdFolder?.Id))
        {
            var errorMessage = _stringLocalizerGoogleDriveStorage[
                nameof(GoogleDriveStorageSharedResource_en.CreatedFolderIdNotReturned)
            ].Value;

            throw new InvalidOperationException(errorMessage);
        }

        return createdFolder.Id;
    }

    private static async Task<string?> FindFolderIdAsync(
        DriveService driveService,
        string? parentFolderId,
        string folderName,
        CancellationToken cancellationToken)
    {
        var listRequest = driveService.Files.List();
        listRequest.Q = BuildFolderQuery(folderName, parentFolderId);
        listRequest.Fields = "files(id,name)";
        listRequest.PageSize = 1;
        listRequest.Spaces = "drive";
        listRequest.SupportsAllDrives = true;
        listRequest.IncludeItemsFromAllDrives = true;

        var existingFolders = await listRequest.ExecuteAsync(cancellationToken);

        return existingFolders.Files is { Count: > 0 }
            ? existingFolders.Files[0].Id
            : null;
    }
}
