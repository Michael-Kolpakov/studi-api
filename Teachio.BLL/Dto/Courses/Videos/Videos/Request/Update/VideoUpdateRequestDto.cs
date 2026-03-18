using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;

/// <summary>
/// Represents the <see cref="VideoUpdateRequestDto"/> type.
/// </summary>
public class VideoUpdateRequestDto : VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
