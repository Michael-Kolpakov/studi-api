namespace Teachio.BLL.DTOs.Courses.Courses.Response;

public class CoursePreviewShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int VideosCount { get; set; }

    public float TotalDuration { get; set; }

    public string? ThumbnailRelativePath { get; set; }

    public int WatchingUsersCount { get; set; }
}
