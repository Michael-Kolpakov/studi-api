using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Create;

public class VideoProgressCreateRequestDto : VideoProgressCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid VideoId { get; set; }
}
