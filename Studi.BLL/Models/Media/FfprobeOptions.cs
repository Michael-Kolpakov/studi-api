namespace Studi.BLL.Models.Media;

public class FfprobeOptions
{
    public const string SectionName = "Ffprobe";

    public string ExecutablePath { get; set; } = "ffprobe";

    public int AnalysisTimeoutSeconds { get; set; } = 30;
}
