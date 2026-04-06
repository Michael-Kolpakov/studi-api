using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.Dto.Courses.Courses.Response;

public class CourseResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int SectionsCount { get; set; }

    public float TotalDuration { get; set; }

    public int WatchingUsersCount { get; set; }

    public VideoResponseDto? SelectedVideo { get; set; }

    public List<SectionShortResponseDto> Sections { get; set; } = new List<SectionShortResponseDto>();
}
