using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.Dto.Courses.Courses.Response;

public class CoursePreviewResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int SectionsCount { get; set; }

    public float TotalDuration { get; set; }

    public string? ThumbnailName { get; set; }

    public Guid OwnerUserId { get; set; }

    public List<SectionShortResponseDto>? Sections { get; set; }
}
