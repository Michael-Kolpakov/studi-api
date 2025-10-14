using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Update;

public class VideoUpdateDto : VideoCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid Id { get; set; }
}
