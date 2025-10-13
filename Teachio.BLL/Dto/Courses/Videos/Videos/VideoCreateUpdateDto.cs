namespace Teachio.BLL.Dto.Courses.Videos.Videos;

public abstract class VideoCreateUpdateDto
{
    public string? Title { get; set; }

    public string? OriginalFileName { get; set; }

    public Guid SectionId { get; set; }

    public int? OrderIndex { get; set; }
}
