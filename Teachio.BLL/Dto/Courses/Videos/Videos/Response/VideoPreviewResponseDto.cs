namespace Teachio.BLL.Dto.Courses.Videos.Videos.Response;

public class VideoPreviewResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int DurationSeconds { get; set; }
}
