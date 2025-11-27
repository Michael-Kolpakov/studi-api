using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.VideoProgress;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Persistence;

public class TeachioDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public TeachioDbContext()
    {
    }

    public TeachioDbContext(DbContextOptions<TeachioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Section> Sections { get; set; } = null!;

    public DbSet<Video> Videos { get; set; } = null!;

    public DbSet<VideoProgress> VideoProgress { get; set; } = null!;

    public DbSet<AppUser> AppUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureCourses();
        builder.ConfigureSections();
        builder.ConfigureVideos();
        builder.ConfigureVideoProgress();
        builder.ConfigureAppUsers();
    }
}
