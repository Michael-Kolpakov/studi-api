using Teachio.BLL.Dto.Courses.Videos;

namespace Teachio.BLL.Dto.Courses.Sections;

public class SectionResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public int OrderIndex { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoResponseDto>? Videos { get; set; }
}
