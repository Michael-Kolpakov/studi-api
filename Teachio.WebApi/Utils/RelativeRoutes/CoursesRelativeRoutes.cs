namespace Teachio.WebApi.Utils.RelativeRoutes;

/// <summary>
/// Represents the <see cref="CoursesRelativeRoutes"/> type.
/// </summary>
public static class CoursesRelativeRoutes
{
    public const string GetPaginated = "get-paginated";

    public const string GetById = "get-by-id/{id:guid}";

    public const string GetByIdPreview = "get-by-id-preview/{id:guid}";

    public const string Create = "create";

    public const string UploadThumbnail = "upload-thumbnail";

    public const string Update = "update";

    public const string Delete = "delete/{id:guid}";
}
