using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.DTOs.Users.Account.Response;

public class AppUserResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public List<CourseResponseDto> OwnedCourses { get; set; } = new List<CourseResponseDto>();

    public List<CourseResponseDto> WatchingCourses { get; set; } = new List<CourseResponseDto>();
}
