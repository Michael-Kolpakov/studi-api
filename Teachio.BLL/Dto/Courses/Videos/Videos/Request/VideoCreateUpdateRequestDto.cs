using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request;

public abstract class VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(EntityConstants.MaxVideoTitleLength, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Range(EntityConstants.MinNonNegativeValue, EntityConstants.MaxVideosPerSection - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
