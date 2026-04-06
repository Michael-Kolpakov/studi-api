using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Entities.Shared;

public class UserCourse
{
    public Guid AppUserId { get; set; }

    public AppUser AppUser { get; set; } = null!;

    public Guid CourseId { get; set; }

    public Course Course { get; set; } = null!;
}
