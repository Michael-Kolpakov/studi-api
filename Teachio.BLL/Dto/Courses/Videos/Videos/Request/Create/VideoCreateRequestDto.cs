using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;

/// <summary>
/// Represents the <see cref="VideoCreateRequestDto"/> type.
/// </summary>
public class VideoCreateRequestDto : VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid SectionId { get; set; }
}
