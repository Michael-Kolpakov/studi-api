namespace Teachio.WebApi.Utils.RelativeRoutes;

public static class VideosRelativeRoutes
{
    public const string GetBySectionId = "sections/{sectionId:guid}/videos";

    public const string GetById = "get-by-id/{id:guid}";

    public const string Stream = "stream/{id:guid}";

    public const string Create = "create";

    public const string Upload = "upload";

    public const string Update = "update";

    public const string UpdateOrderIndex = "update-order-index";

    public const string Delete = "delete/{id:guid}";
}
