using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.DTOs.Courses.Courses.Response;

public class CourseResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string OwnerFullName { get; set; } = null!;

    public int SectionsCount { get; set; }

    public float TotalDurationHours { get; set; }

    public int WatchingUsersCount { get; set; }

    public VideoResponseDto? SelectedVideo { get; set; }

    public List<SectionShortResponseDto> Sections { get; set; } = new List<SectionShortResponseDto>();
}
