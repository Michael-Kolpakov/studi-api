namespace Teachio.BLL.Utils.Helpers;

public static class StoragePathHelper
{
    #region Thumbnails

    public static IReadOnlyList<string> BuildDefaultThumbnailFolderSegments()
    {
        return new List<string>
        {
            "defaults",
            "thumbnails"
        };
    }

    public static IReadOnlyList<string> BuildThumbnailFolderSegments(
        string ownerUserEmail,
        string courseName)
    {
        return new List<string>
        {
            "users",
            ownerUserEmail,
            "courses",
            courseName
        };
    }

    #endregion

    #region Videos

    public static IReadOnlyList<string> BuildVideoFolderSegments(
        string ownerUserEmail,
        string courseName,
        string sectionName)
    {
        return new List<string>
        {
            "users",
            ownerUserEmail,
            "courses",
            courseName,
            "sections",
            sectionName
        };
    }

    #endregion
}
