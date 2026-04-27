namespace Teachio.BLL.Models.Media;

public class VideoFileMetadata
{
    public string ContentType { get; set; } = null!;

    public string FileExtension { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public int Resolution { get; set; }
}
