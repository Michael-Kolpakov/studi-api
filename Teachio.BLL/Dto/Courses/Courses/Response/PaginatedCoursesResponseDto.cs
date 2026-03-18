namespace Teachio.BLL.Dto.Courses.Courses.Response;

/// <summary>
/// Represents the <see cref="PaginatedCoursesResponseDto"/> type.
/// </summary>
public class PaginatedCoursesResponseDto
{
    public int TotalAmount { get; set; }

    public IEnumerable<CoursePreviewShortResponseDto> Courses { get; set; } = new List<CoursePreviewShortResponseDto>();
}
