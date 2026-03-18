using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.Dto.Courses.Courses.Response;

/// <summary>
/// Represents the <see cref="CoursePreviewResponseDto"/> type.
/// </summary>
public class CoursePreviewResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int SectionsCount { get; set; }

    public float TotalDuration { get; set; }

    public string ThumbnailRelativePath { get; set; } = null!;

    public int WatchingUsersCount { get; set; }

    public List<SectionPreviewResponseDto> Sections { get; set; } = new List<SectionPreviewResponseDto>();
}
