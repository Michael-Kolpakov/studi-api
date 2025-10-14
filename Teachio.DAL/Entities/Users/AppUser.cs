using Microsoft.AspNetCore.Identity;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.DAL.Entities.Users;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public Role Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<Course> OwnedCourses { get; set; } = new List<Course>();

    public List<Course> WatchingCourses { get; set; } = new List<Course>();
}

public enum Role
{
    ContentCreator,
    Watcher
}
