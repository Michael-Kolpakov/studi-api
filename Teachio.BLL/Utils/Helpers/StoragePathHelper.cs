namespace Teachio.BLL.Utils.Helpers;

public static class StoragePathHelper
{
    public static IReadOnlyList<string> BuildThumbnailFolderSegments(
        string ownerUserEmail,
        string courseName)
    {
        return new List<string>
        {
            ownerUserEmail,
            "courses",
            courseName
        };
    }

    public static IReadOnlyList<string> BuildVideoFolderSegments(
        string ownerUserEmail,
        string courseName,
        string sectionName)
    {
        return new List<string>
        {
            ownerUserEmail,
            "courses",
            courseName,
            "sections",
            sectionName
        };
    }
}
