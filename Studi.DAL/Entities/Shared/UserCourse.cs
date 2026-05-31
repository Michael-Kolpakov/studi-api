using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Users.Users;

namespace Studi.DAL.Entities.Shared;

public class UserCourse
{
    public Guid AppUserId { get; set; }

    public AppUser AppUser { get; set; } = null!;

    public Guid CourseId { get; set; }

    public Course Course { get; set; } = null!;
}
