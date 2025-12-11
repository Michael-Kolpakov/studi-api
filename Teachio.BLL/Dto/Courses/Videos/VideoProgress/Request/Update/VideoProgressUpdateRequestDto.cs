using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;

public class VideoProgressUpdateRequestDto : VideoProgressCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
