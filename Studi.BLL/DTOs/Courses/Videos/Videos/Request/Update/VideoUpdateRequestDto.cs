using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Courses.Videos.Videos.Request.Update;

public class VideoUpdateRequestDto : VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
