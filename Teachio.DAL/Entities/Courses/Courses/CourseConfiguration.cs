using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.ThumbnailFiles;
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
            .ToTable($"{nameof(Course)}s", DatabaseConstants.CoursesSchema)
            .HasKey(c => c.Id);

        builder.Entity<Course>(typeBuilder =>
        {
            typeBuilder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxCourseTitleLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.Title)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Course.Title), ValidationRule.Title)));

            typeBuilder.Property(c => c.Description)
                .HasMaxLength(EntityConstants.MaxCourseDescriptionLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.Description)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Course.Description), ValidationRule.Description)));

            typeBuilder.Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxCourseNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.CourseName)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Course.CourseName), ValidationRule.Name)));

            typeBuilder.Property(s => s.SectionsCount)
                .IsRequired()
                .HasDefaultValue(EntityConstants.MinNonNegativeValue);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.SectionsCount)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(Course.SectionsCount),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxSectionsPerCourse)));

            typeBuilder.Property(s => s.WatchingUsersCount)
                .IsRequired()
                .HasDefaultValue(EntityConstants.MinNonNegativeValue);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Course)}_{nameof(Course.WatchingUsersCount)}_{nameof(CheckConstraintType.NonNegative)}",
                    ConstraintHelper.CreateSqlNonNegativeCheck(nameof(Course.WatchingUsersCount))));

            typeBuilder.Property(c => c.OwnerUserId)
                .IsRequired();

            typeBuilder.Property(c => c.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });

        builder.Entity<Course>()
            .HasOne<AppUser>(course => course.OwnerUser)
            .WithMany(appUser => appUser.OwnedCourses)
            .HasForeignKey(course => course.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Course>()
            .HasOne<ThumbnailFile>(course => course.ThumbnailFile)
            .WithOne(thumbnailFile => thumbnailFile.Course)
            .HasForeignKey<ThumbnailFile>(thumbnailFile => thumbnailFile.CourseId)
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
                    j.ToTable(DatabaseConstants.UserCourseTableName, DatabaseConstants.CoursesSchema);
                });
    }
}
