namespace Studi.BLL.Utils.Helpers;

public static class VideoNameHelper
{
    public static string GetBaseName(string? videoName)
    {
        if (string.IsNullOrWhiteSpace(videoName))
        {
            return string.Empty;
        }

        return Path.GetFileNameWithoutExtension(videoName);
    }

    public static bool HasSameBaseName(string? videoName, string candidateBaseName)
    {
        return string.Equals(
            GetBaseName(videoName),
            candidateBaseName,
            StringComparison.OrdinalIgnoreCase);
    }
}
