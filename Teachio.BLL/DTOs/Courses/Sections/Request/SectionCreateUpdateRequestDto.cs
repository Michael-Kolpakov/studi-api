using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Courses.Sections.Request;

public abstract class SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(EntityConstants.MaxSectionTitleLength, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Range(EntityConstants.MinNonNegativeValue, EntityConstants.MaxSectionsPerCourse - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
