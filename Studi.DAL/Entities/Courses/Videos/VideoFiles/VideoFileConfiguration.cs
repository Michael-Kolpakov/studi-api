using Microsoft.EntityFrameworkCore;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Helpers;

namespace Studi.DAL.Entities.Courses.Videos.VideoFiles;

public static class VideoFileConfiguration
{
    public static void ConfigureVideoFiles(this ModelBuilder builder)
    {
        builder.Entity<VideoFile>()
            .ToTable($"{nameof(VideoFile)}s", DatabaseConstants.CoursesSchema)
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
                    ConstraintHelper.CreateSqlRegexCheck(nameof(VideoFile.VideoName), ValidationRule.MediaName)));

            typeBuilder.Property(v => v.ContentType)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxMediaContentTypeLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.ContentType)}_{nameof(CheckConstraintType.AllowedValues)}",
                    ConstraintHelper.CreateSqlAllowedValuesCheck(nameof(VideoFile.ContentType), EntityConstants.AllowedVideoContentTypes)));

            typeBuilder.Property(v => v.DurationSeconds)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.DurationSeconds)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(VideoFile.DurationSeconds),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxVideoDurationSeconds)));

            typeBuilder.Property(v => v.Resolution)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoFile)}_{nameof(VideoFile.Resolution)}_{nameof(CheckConstraintType.AllowedValues)}",
                    ConstraintHelper.CreateSqlAllowedValuesCheck(nameof(VideoFile.Resolution), EntityConstants.AllowedVideoResolutions)));

            typeBuilder.Property(vp => vp.VideoId)
                .IsRequired();

            typeBuilder.Property(v => v.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(v => v.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });
    }
}
