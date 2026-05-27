namespace Studi.BLL.Models.Media;

public class AvatarFileMetadata
{
    public string ContentType { get; set; } = null!;

    public string FileExtension { get; set; } = null!;

    public string Resolution { get; set; } = null!;

    public int Width { get; set; }

    public int Height { get; set; }
}
