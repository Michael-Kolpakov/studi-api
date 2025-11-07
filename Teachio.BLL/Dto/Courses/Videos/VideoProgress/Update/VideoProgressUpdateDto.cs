using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Update;

public class VideoProgressUpdateDto : VideoProgressCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
