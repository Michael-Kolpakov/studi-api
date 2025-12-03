namespace Teachio.WebApi.Utils.RelativeRoutes;

public static class CoursesRelativeRoutes
{
    public const string GetPaginated = "get-paginated";

    public const string GetById = "get-by-id/{id:guid}";

    public const string GetByIdPreview = "get-by-id-preview/{id:guid}";

    public const string Create = "create";

    public const string Update = "update";

    public const string Delete = "delete/{id:guid}";
}
