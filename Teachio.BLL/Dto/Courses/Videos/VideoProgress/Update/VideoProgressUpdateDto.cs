using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Update;

public class VideoProgressUpdateDto : VideoProgressCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid Id { get; set; }
}
