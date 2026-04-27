namespace Teachio.BLL.Models.Storage;

public class GoogleDriveStorageOptions
{
    public const string SectionName = "GoogleDriveStorage";

    public const string DefaultOAuthUserId = "teachio-google-drive";

    public string ApplicationName { get; set; } = "Teachio.WebApi";

    public string RootFolderName { get; set; } = "move-it-storage";

    public string? RootFolderId { get; set; }

    public string? OAuthClientId { get; set; }

    public string? OAuthClientSecret { get; set; }

    public string? OAuthRefreshToken { get; set; }

    public string OAuthUserId { get; set; } = DefaultOAuthUserId;

    public string? ServiceAccountCredentialPath { get; set; }

    public string? ServiceAccountCredentialJson { get; set; }
}
