using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Videos.Videos;

public static class VideoConfiguration
{
    public static void ConfigureVideos(this ModelBuilder builder)
    {
        builder.Entity<Video>()
            .ToTable($"{nameof(Video)}s", $"{nameof(Course).ToLowerInvariant()}s")
            .HasKey(v => v.Id);

        builder.Entity<Video>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.Title)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Video.Title), ValidationRule.Title)));

            typeBuilder.Property(v => v.VideoName)
                .IsRequired()
                .HasMaxLength(110);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.VideoName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Video.VideoName), ValidationRule.MediaName)));

            typeBuilder.Property(v => v.ContentType)
                .HasMaxLength(100);

            typeBuilder.Property(v => v.SectionId)
                .IsRequired();

            typeBuilder.Property(v => v.OrderIndex)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.OrderIndex)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(Video.OrderIndex), 0, EntityConstants.MaxVideosPerSection - 1)));

            typeBuilder.Property(v => v.DurationSeconds)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.DurationSeconds)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(Video.DurationSeconds), 0, EntityConstants.MaxVideoDurationSeconds)));

            typeBuilder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>();

            typeBuilder.Property(v => v.ProcessingError)
                .HasMaxLength(100);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.ProcessingError)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Video.ProcessingError), ValidationRule.ProcessingError)));

            typeBuilder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });

        builder.Entity<Video>()
            .HasOne<VideoProgress.VideoProgress>(video => video.VideoProgress)
            .WithOne(videoProgress => videoProgress.Video)
            .HasForeignKey<VideoProgress.VideoProgress>(videoProgress => videoProgress.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
