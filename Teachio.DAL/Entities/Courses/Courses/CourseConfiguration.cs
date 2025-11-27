using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Users;
using Teachio.DAL.Shared;

namespace Teachio.DAL.Entities.Courses.Courses;

public static class CourseConfiguration
{
    public static void ConfigureCourses(this ModelBuilder builder)
    {
        builder.Entity<Course>()
            .ToTable("Courses", "courses")
            .HasKey(c => c.Id);

        builder.Entity<Course>(typeBuilder =>
        {
            typeBuilder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.Property(c => c.Description)
                .HasMaxLength(1000);

            typeBuilder.Property(c => c.ThumbnailName)
                .IsRequired()
                .HasMaxLength(110);

            typeBuilder.Property(s => s.SectionsCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Course_SectionsCount_Max",
                    $"[SectionsCount] >= 0 AND [SectionsCount] <= {EntityConstants.MaxSectionsPerCourse}"));

            typeBuilder.Property(c => c.OwnerUserId)
                .IsRequired();

            typeBuilder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });

        builder.Entity<Course>()
            .HasOne<AppUser>(course => course.OwnerUser)
            .WithMany(appUser => appUser.OwnedCourses)
            .HasForeignKey(course => course.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Course>()
            .HasMany<Section>(course => course.Sections)
            .WithOne(section => section.Course)
            .HasForeignKey(section => section.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Course>()
            .HasMany(course => course.WatchingUsers)
            .WithMany(appUser => appUser.WatchingCourses)
            .UsingEntity<Dictionary<string, object>>(
                "UserCourse",
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("AppUserId")
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne<Course>()
                    .WithMany()
                    .HasForeignKey("CourseId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("AppUserId", "CourseId");
                    j.ToTable("UserCourse", "courses");
                });
    }
}
