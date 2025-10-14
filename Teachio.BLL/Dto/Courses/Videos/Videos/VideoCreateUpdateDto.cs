using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Videos.Videos;

public abstract class VideoCreateUpdateDto
{
    public string? Title { get; set; }

    public Guid SectionId { get; set; }

    public int? OrderIndex { get; set; }

    public IFormFile File { get; set; } = null!;
}
