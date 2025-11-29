using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.Dto.Courses.Sections.Response;

public class SectionShortResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoShortResponseDto>? Videos { get; set; }
}
