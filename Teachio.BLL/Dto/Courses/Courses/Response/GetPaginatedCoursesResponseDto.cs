namespace Teachio.BLL.Dto.Courses.Courses.Response;

public class GetPaginatedCoursesResponseDto
{
    public int TotalAmount { get; set; }

    public IEnumerable<CoursePreviewShortResponseDto> Courses { get; set; } = new List<CoursePreviewShortResponseDto>();
}
