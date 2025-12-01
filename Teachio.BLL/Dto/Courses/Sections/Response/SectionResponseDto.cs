using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.Dto.Courses.Sections.Response;

public class SectionResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoResponseDto> Videos { get; set; } = new List<VideoResponseDto>();
}
