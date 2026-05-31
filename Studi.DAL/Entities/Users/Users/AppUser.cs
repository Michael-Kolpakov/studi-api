using Microsoft.AspNetCore.Identity;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Videos.VideoProgress;
using Studi.DAL.Entities.Users.AvatarFiles;

namespace Studi.DAL.Entities.Users.Users;

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
