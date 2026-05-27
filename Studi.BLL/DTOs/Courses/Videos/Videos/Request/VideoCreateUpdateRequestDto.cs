using System.ComponentModel.DataAnnotations;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Courses.Videos.Videos.Request;

public abstract class VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(EntityConstants.MaxVideoTitleLength, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [StringLength(EntityConstants.MaxVideoDescriptionLength, ErrorMessage = "Length")]
    [RegularExpression(EntityConstants.DescriptionRegexPattern, ErrorMessage = "RegexDescription")]
    public string? Description { get; set; }
}
