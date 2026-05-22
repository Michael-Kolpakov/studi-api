using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.DTOs.Courses.Courses.Response;

public class CoursePreviewResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string OwnerFullName { get; set; } = null!;

    public int SectionsCount { get; set; }

    public float TotalDurationHours { get; set; }

    public string? ThumbnailRelativePath { get; set; }

    public int WatchingUsersCount { get; set; }

    public List<SectionPreviewResponseDto> Sections { get; set; } = new List<SectionPreviewResponseDto>();
}
