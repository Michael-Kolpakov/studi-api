using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request;

public abstract class VideoProgressCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid VideoId { get; set; }

    [Required(ErrorMessage = "Required")]
    public bool IsCompleted { get; set; }

    [Required(ErrorMessage = "Required")]
    public int PositionSeconds { get; set; }
}
