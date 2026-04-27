using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Videos.VideoFiles;

public static class VideoFileConfiguration
{
    public static void ConfigureVideoFiles(this ModelBuilder builder)
    {
        builder.Entity<VideoFile>()
            .ToTable($"{nameof(VideoFile)}s", $"{nameof(Course).ToLowerInvariant()}s")
            .HasKey(v => v.Id);

        builder.Entity<VideoFile>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.VideoName)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxVideoFileNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.VideoName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(VideoFile.VideoName), ValidationRule.MediaName)));

            typeBuilder.Property(v => v.ContentType)
                .IsRequired()
                .HasMaxLength(20);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.ContentType)}_{nameof(CheckConstraintType.AllowedValues)}",
                    Constraint.CreateSqlAllowedValuesCheck(nameof(VideoFile.ContentType), EntityConstants.AllowedVideoContentTypes)));

            typeBuilder.Property(v => v.DurationSeconds)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.DurationSeconds)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(VideoFile.DurationSeconds), 0, EntityConstants.MaxVideoDurationSeconds)));

            typeBuilder.Property(v => v.Resolution)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.Resolution)}_{nameof(CheckConstraintType.AllowedValues)}",
                    Constraint.CreateSqlAllowedValuesCheck(nameof(VideoFile.Resolution), EntityConstants.AllowedVideoResolutions)));

            typeBuilder.Property(vp => vp.VideoId)
                .IsRequired();

            typeBuilder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            typeBuilder.Property(v => v.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });
    }
}
