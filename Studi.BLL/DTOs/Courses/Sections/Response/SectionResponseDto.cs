using Studi.BLL.DTOs.Courses.Videos.Videos.Response;

namespace Studi.BLL.DTOs.Courses.Sections.Response;

public class SectionResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public float TotalDurationSeconds { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoResponseDto> Videos { get; set; } = new List<VideoResponseDto>();
}
