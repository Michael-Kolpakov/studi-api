using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Update;

public class VideoUpdateDto : VideoCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
