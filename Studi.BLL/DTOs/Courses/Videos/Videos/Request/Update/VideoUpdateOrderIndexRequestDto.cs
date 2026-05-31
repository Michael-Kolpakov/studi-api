using System.ComponentModel.DataAnnotations;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Courses.Videos.Videos.Request.Update;

public class VideoUpdateOrderIndexRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(EntityConstants.MinNonNegativeValue, EntityConstants.MaxVideosPerSection - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
