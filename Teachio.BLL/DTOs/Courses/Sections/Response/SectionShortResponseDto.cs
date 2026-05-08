using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Teachio.BLL.DTOs.Courses.Sections.Response;

public class SectionShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public float TotalDurationSeconds { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoShortResponseDto> Videos { get; set; } = new List<VideoShortResponseDto>();
}
