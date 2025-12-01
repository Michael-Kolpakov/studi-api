namespace Teachio.BLL.Dto.Courses.Courses.Response;

public class CoursePreviewShortResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int VideosCount { get; set; }

    public float TotalDuration { get; set; }

    public string? ThumbnailName { get; set; }

    public int WatchingUsersCount { get; set; }

    public Guid OwnerUserId { get; set; }
}
