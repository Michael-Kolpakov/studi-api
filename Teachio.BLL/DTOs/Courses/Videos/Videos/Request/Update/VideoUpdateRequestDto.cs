using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Update;

public class VideoUpdateRequestDto : VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
