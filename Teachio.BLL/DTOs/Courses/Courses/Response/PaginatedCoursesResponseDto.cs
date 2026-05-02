namespace Teachio.BLL.DTOs.Courses.Courses.Response;

public class PaginatedCoursesResponseDto
{
    public int TotalAmount { get; set; }

    public IEnumerable<CoursePreviewShortResponseDto> Courses { get; set; } = new List<CoursePreviewShortResponseDto>();
}
