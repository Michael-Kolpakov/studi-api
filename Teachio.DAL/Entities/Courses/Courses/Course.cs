using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Entities.Courses.Courses;

public class Course
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ThumbnailName { get; set; } = null!;

    public int SectionsCount { get; set; }

    public Guid OwnerUserId { get; set; }

    public AppUser? OwnerUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<Section> Sections { get; set; } = new List<Section>();

    public List<AppUser> WatchingUsers { get; set; } = new List<AppUser>();
}
