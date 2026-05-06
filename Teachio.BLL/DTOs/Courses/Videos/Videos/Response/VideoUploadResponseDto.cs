namespace Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

public class VideoUploadResponseDto
{
    public Guid Id { get; set; }

    public string VideoName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public int Resolution { get; set; }

    public Guid VideoId { get; set; }
}
