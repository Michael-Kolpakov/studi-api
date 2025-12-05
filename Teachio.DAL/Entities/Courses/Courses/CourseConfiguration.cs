using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Shared;
using Teachio.DAL.Entities.Users;
using Teachio.DAL.Utils.Constants;

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

            typeBuilder.Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(60);

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

            typeBuilder.Property(s => s.WatchingUsersCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Course_WatchingUsersCount_NonNegative",
                    $"[WatchingUsersCount] >= 0"));

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
            .UsingEntity<UserCourse>(
                j => j
                    .HasOne(userCourse => userCourse.AppUser)
                    .WithMany()
                    .HasForeignKey(userCourse => userCourse.AppUserId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(userCourse => userCourse.Course)
                    .WithMany()
                    .HasForeignKey(userCourse => userCourse.CourseId)
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey(userCourse => new { userCourse.AppUserId, userCourse.CourseId });
                    j.ToTable("UserCourse", "courses");
                });
    }
}
