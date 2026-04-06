using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.Dto.Users.Response;

public class AppUserResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public List<CourseResponseDto> OwnedCourses { get; set; } = new List<CourseResponseDto>();

    public List<CourseResponseDto> WatchingCourses { get; set; } = new List<CourseResponseDto>();
}
