using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Courses.Request;

public abstract class CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(EntityConstants.MaxCourseTitleLength, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [StringLength(EntityConstants.MaxCourseDescriptionLength, ErrorMessage = "Length")]
    [RegularExpression(EntityConstants.DescriptionRegexPattern, ErrorMessage = "RegexDescription")]
    public string? Description { get; set; }
}
