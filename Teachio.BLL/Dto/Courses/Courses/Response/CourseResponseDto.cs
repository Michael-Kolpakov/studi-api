using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Dto.Users;
using Teachio.BLL.Dto.Users.Response;

namespace Teachio.BLL.Dto.Courses.Courses.Response;

public class CourseResponseDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Guid OwnerUserId { get; set; }

    public List<SectionResponseDto>? Sections { get; set; }
    
    public List<AppUserResponseDto>? WatchingUsers { get; set; }
}
