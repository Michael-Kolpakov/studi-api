using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request;

/// <summary>
/// Represents the <see cref="VideoCreateUpdateRequestDto"/> type.
/// </summary>
public abstract class VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.TitleRegexPattern, ErrorMessage = "RegexTitle")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Range(0, EntityConstants.MaxVideosPerSection - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }

    [Required(ErrorMessage = "Required")]
    public IFormFile VideoFile { get; set; } = null!;
}
