using System.ComponentModel.DataAnnotations;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Courses.Sections.Request;

public abstract class SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(EntityConstants.MaxSectionTitleLength, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;
}
