namespace Teachio.BLL.Utils.Helpers;

public static class ThumbnailStoragePathHelper
{
    public static IReadOnlyList<string> BuildThumbnailFolderSegments(
        string ownerUserEmail,
        string courseName)
    {
        return
        [
            ownerUserEmail,
            "courses",
            courseName
        ];
    }
}
