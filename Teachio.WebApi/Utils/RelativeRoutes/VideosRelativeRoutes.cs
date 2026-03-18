namespace Teachio.WebApi.Utils.RelativeRoutes;

/// <summary>
/// Represents the <see cref="VideosRelativeRoutes"/> type.
/// </summary>
public static class VideosRelativeRoutes
{
    public const string GetById = "get-by-id/{id:guid}";

    public const string Create = "create";

    public const string UploadVideo = "upload-video";

    public const string Update = "update";

    public const string Delete = "delete/{id:guid}";
}
