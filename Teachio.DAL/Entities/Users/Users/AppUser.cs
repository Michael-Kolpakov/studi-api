using Microsoft.AspNetCore.Identity;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.VideoProgress;
using Teachio.DAL.Entities.Users.AvatarFiles;

namespace Teachio.DAL.Entities.Users.Users;

public class AppUser : IdentityUser<Guid>
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenCreatedAt { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }

    public DateTime? RefreshTokenRevokedAt { get; set; }

    public AvatarFile? AvatarFile { get; set; }

    public List<Course> OwnedCourses { get; set; } = new List<Course>();

    public List<Course> WatchingCourses { get; set; } = new List<Course>();

    public List<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
}
