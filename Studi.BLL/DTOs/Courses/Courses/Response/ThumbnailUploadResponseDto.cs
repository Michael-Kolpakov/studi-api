namespace Studi.BLL.DTOs.Courses.Courses.Response;

public class ThumbnailUploadResponseDto
{
    public Guid Id { get; set; }

    public string ThumbnailName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string Resolution { get; set; } = null!;

    public Guid CourseId { get; set; }
}
