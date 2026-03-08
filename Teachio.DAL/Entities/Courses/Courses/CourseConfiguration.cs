using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Shared;
using Teachio.DAL.Entities.Users;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Courses;

public static class CourseConfiguration
{
    public static void ConfigureCourses(this ModelBuilder builder)
    {
        builder.Entity<Course>()
            .ToTable($"{nameof(Course)}s", $"{nameof(Course).ToLowerInvariant()}s")
            .HasKey(c => c.Id);

        builder.Entity<Course>(typeBuilder =>
        {
            typeBuilder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.Title)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Course.Title), ValidationRule.Title)));

            typeBuilder.Property(c => c.Description)
                .HasMaxLength(1000);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.Description)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Course.Description), ValidationRule.Description)));

            typeBuilder.Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.CourseName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Course.CourseName), ValidationRule.Name)));

            typeBuilder.Property(c => c.ThumbnailName)
                .IsRequired()
                .HasMaxLength(110);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.ThumbnailName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Course.ThumbnailName), ValidationRule.MediaName)));

            typeBuilder.Property(s => s.SectionsCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.SectionsCount)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(Course.SectionsCount), 0, EntityConstants.MaxSectionsPerCourse)));

            typeBuilder.Property(s => s.WatchingUsersCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.WatchingUsersCount)}_{nameof(CheckConstraintType.NonNegative)}",
                    Constraint.CreateSqlNonNegativeCheck(nameof(Course.WatchingUsersCount))));

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
