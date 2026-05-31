namespace Studi.BLL.DTOs.Courses.Courses.Response;

public class CoursePreviewShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string OwnerFullName { get; set; } = null!;

    public int VideosCount { get; set; }

    public float TotalDurationHours { get; set; }

    public int WatchingUsersCount { get; set; }
}
