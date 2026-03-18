using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Entities.Shared;

/// <summary>
/// Represents the <see cref="UserCourse"/> type.
/// </summary>
public class UserCourse
{
    public Guid AppUserId { get; set; }

    public AppUser AppUser { get; set; } = null!;

    public Guid CourseId { get; set; }

    public Course Course { get; set; } = null!;
}
