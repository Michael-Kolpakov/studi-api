namespace Teachio.BLL.Utils.Helpers;

public static class VideoStoragePathHelper
{
    public static IReadOnlyList<string> BuildVideoFolderSegments(
        string ownerUserEmail,
        string courseName,
        string sectionName)
    {
        return
        [
            ownerUserEmail,
            "courses",
            courseName,
            "sections",
            sectionName
        ];
    }
}
