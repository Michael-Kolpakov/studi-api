using Teachio.BLL.Dto.Courses.Courses;

namespace Teachio.BLL.Dto.Users;

public class AppUserResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public List<CourseResponseDto>? OwnedCourses { get; set; }

    public List<CourseResponseDto>? WatchingCourses { get; set; }
}
