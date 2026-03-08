using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;

public class VideoCreateRequestDto : VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid SectionId { get; set; }
}
