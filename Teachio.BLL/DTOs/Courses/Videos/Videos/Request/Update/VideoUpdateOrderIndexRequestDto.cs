using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Courses.Videos.Videos.Request.Update;

public class VideoUpdateOrderIndexRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(EntityConstants.MinNonNegativeValue, EntityConstants.MaxVideosPerSection - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
