namespace Teachio.WebApi.Utils.RelativeRoutes;

public static class SectionsRelativeRoutes
{
    public const string GetByCourseId = "get-by-course-id/{courseId:guid}";

    public const string GetById = "get-by-id/{id:guid}";

    public const string Create = "create";

    public const string Update = "update";

    public const string UpdateOrderIndex = "update-order-index";

    public const string Delete = "delete/{id:guid}";
}
