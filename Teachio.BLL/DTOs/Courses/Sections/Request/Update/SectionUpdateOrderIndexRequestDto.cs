using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Courses.Sections.Request.Update;

public class SectionUpdateOrderIndexRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(EntityConstants.MinNonNegativeValue, EntityConstants.MaxSectionsPerCourse - 1, ErrorMessage = "Range")]
    public int OrderIndex { get; set; }
}
