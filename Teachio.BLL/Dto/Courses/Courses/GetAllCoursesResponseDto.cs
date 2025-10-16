namespace Teachio.BLL.Dto.Courses.Courses;

public class GetAllCoursesResponseDto
{
    public int TotalAmount { get; set; }

    public IEnumerable<CourseResponseDto> Courses { get; set; } = new List<CourseResponseDto>();
}
