namespace Teachio.BLL.Models.Storage;

public class GoogleDriveStreamResult
{
    public Stream ContentStream { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long? ContentLength { get; set; }

    public string? ContentRange { get; set; }

    public bool IsPartialContent { get; set; }
}
