using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;

/// <summary>
/// Represents the <see cref="VideoProgressUpdateRequestDto"/> type.
/// </summary>
public class VideoProgressUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Required")]
    public bool IsCompleted { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(0, EntityConstants.MaxVideoDurationSeconds, ErrorMessage = "Range")]
    public int PositionSeconds { get; set; }
}
