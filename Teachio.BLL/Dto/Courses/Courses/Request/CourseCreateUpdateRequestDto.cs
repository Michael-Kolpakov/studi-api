using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Courses.Request;

/// <summary>
/// Represents the <see cref="CourseCreateUpdateRequestDto"/> type.
/// </summary>
public abstract class CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Length")]
    [RegularExpression(EntityConstants.DescriptionRegexPattern, ErrorMessage = "RegexDescription")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.MediaNameRegexPattern, ErrorMessage = "RegexName")]
    public string ThumbnailName { get; set; } = null!;
}
