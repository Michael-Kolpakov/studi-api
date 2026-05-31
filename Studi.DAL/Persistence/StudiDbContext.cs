using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.ThumbnailFiles;
using Studi.DAL.Entities.Courses.Videos.VideoFiles;
using Studi.DAL.Entities.Courses.Videos.VideoProgress;
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Entities.Shared;
using Studi.DAL.Entities.Users.AvatarFiles;
using Studi.DAL.Entities.Users.PendingRegistrations;
using Studi.DAL.Entities.Users.Users;

namespace Studi.DAL.Persistence;

public class StudiDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private const string CreatedAtPropertyName = nameof(Course.CreatedAt);

    private const string UpdatedAtPropertyName = nameof(Course.UpdatedAt);

    public StudiDbContext()
    {
    }

    public StudiDbContext(DbContextOptions<StudiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<ThumbnailFile> ThumbnailFiles { get; set; } = null!;

    public DbSet<Section> Sections { get; set; } = null!;

    public DbSet<Video> Videos { get; set; } = null!;

    public DbSet<VideoFile> VideoFiles { get; set; } = null!;

    public DbSet<VideoProgress> VideoProgress { get; set; } = null!;

    public DbSet<AppUser> AppUsers { get; set; } = null!;

    public DbSet<AvatarFile> AvatarFiles { get; set; } = null!;

    public DbSet<UserCourse> UserCourses { get; set; } = null!;

    public DbSet<PendingRegistration> PendingRegistrations { get; set; } = null!;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditTimestamps();
        EnsureVideoProgressDeletionInvariant();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        EnsureVideoProgressDeletionInvariant();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureCourses();
        builder.ConfigureThumbnailFiles();
        builder.ConfigureSections();
        builder.ConfigureVideos();
        builder.ConfigureVideoFiles();
        builder.ConfigureVideoProgress();
        builder.ConfigureAppUsers();
        builder.ConfigureAvatarFiles();
        builder.ConfigurePendingRegistrations();
    }

    private void EnsureVideoProgressDeletionInvariant()
    {
        var deletedVideoProgressEntries = ChangeTracker.Entries<VideoProgress>()
            .Where(entry => entry.State == EntityState.Deleted)
            .ToList();

        if (deletedVideoProgressEntries.Count == 0)
        {
            return;
        }

        var deletedVideoIds = ChangeTracker.Entries<Video>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity.Id)
            .ToHashSet();

        var deletedUserIds = ChangeTracker.Entries<AppUser>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity.Id)
            .ToHashSet();

        foreach (var videoProgressEntry in deletedVideoProgressEntries)
        {
            if (deletedVideoIds.Contains(videoProgressEntry.Entity.VideoId))
            {
                continue;
            }

            if (deletedUserIds.Contains(videoProgressEntry.Entity.AppUserId))
            {
                continue;
            }

            throw new InvalidOperationException(
                $"{nameof(VideoProgress)} cannot be deleted unless its parent {nameof(Video)} or owning {nameof(AppUser)} is deleted in the same unit of work.");
        }
    }

    private void ApplyAuditTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        var changedEntries = ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in changedEntries)
        {
            SetDateTimePropertyValue(entry, UpdatedAtPropertyName, utcNow);

            if (entry.State == EntityState.Added)
            {
                SetDateTimePropertyValue(entry, CreatedAtPropertyName, utcNow);

                continue;
            }

            MarkPropertyAsNotModified(entry, CreatedAtPropertyName);
        }
    }

    private static void SetDateTimePropertyValue(EntityEntry entry, string propertyName, DateTime value)
    {
        UpdateDateTimeProperty(
            entry,
            propertyName,
            propertyEntry => propertyEntry.CurrentValue = value);
    }

    private static void MarkPropertyAsNotModified(EntityEntry entry, string propertyName)
    {
        UpdateDateTimeProperty(
            entry,
            propertyName,
            propertyEntry => propertyEntry.IsModified = false);
    }

    private static void UpdateDateTimeProperty(
        EntityEntry entry,
        string propertyName,
        Action<PropertyEntry> updateAction)
    {
        var property = entry.Metadata.FindProperty(propertyName);

        if (property?.ClrType != typeof(DateTime))
        {
            return;
        }

        updateAction(entry.Property(propertyName));
    }
}
