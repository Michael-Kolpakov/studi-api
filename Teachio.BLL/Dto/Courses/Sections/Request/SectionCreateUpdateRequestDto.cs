using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Sections.Request;

/// <summary>
/// Represents the <see cref="SectionCreateUpdateRequestDto"/> type.
/// </summary>
public abstract class SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Range(0, EntityConstants.MaxSectionsPerCourse - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
