namespace Teachio.BLL.Dto.Courses.Courses.Response;

/// <summary>
/// Represents the <see cref="CoursePreviewShortResponseDto"/> type.
/// </summary>
public class CoursePreviewShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int VideosCount { get; set; }

    public float TotalDuration { get; set; }

    public string ThumbnailRelativePath { get; set; } = null!;

    public int WatchingUsersCount { get; set; }
}
