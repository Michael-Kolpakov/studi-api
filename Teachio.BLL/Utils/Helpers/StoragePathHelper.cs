namespace Teachio.BLL.Utils.Helpers;

public static class StoragePathHelper
{
    #region Avatars

    public static IReadOnlyList<string> BuildUserFolderSegments(string ownerUserEmail)
    {
        return new List<string>
        {
            "users",
            ownerUserEmail
        };
    }

    public static IReadOnlyList<string> BuildDefaultAvatarFolderSegments()
    {
        return new List<string>
        {
            "defaults",
            "avatars"
        };
    }

    #endregion

    #region Thumbnails

    public static IReadOnlyList<string> BuildDefaultThumbnailFolderSegments()
    {
        return new List<string>
        {
            "defaults",
            "thumbnails"
        };
    }

    public static IReadOnlyList<string> BuildCourseFolderSegments(
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

    public static IReadOnlyList<string> BuildSectionFolderSegments(
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
