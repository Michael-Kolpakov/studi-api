using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request;

public abstract class VideoProgressCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public bool IsCompleted { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(0, EntityConstants.MaxVideoDurationSeconds, ErrorMessage = "Range")]
    public int PositionSeconds { get; set; }
}
