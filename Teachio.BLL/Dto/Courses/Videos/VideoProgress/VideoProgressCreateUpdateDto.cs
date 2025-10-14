using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress;

public abstract class VideoProgressCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid VideoId { get; set; }

    [Required(ErrorMessage = "'{0}' field is required")]
    public int PositionSeconds { get; set; }
}
