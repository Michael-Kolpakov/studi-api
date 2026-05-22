using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Videos.VideoFiles;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Videos.Videos;

public static class VideoConfiguration
{
    public static void ConfigureVideos(this ModelBuilder builder)
    {
        builder.Entity<Video>()
            .ToTable($"{nameof(Video)}s", DatabaseConstants.CoursesSchema)
            .HasKey(v => v.Id);

        builder.Entity<Video>()
            .HasIndex(v => new { v.SectionId, v.OrderIndex })
            .IsUnique();

        builder.Entity<Video>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxVideoTitleLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.Title)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Video.Title), ValidationRule.Title)));

            typeBuilder.Property(v => v.SectionId)
                .IsRequired();

            typeBuilder.Property(v => v.OrderIndex)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Video)}_{nameof(Video.OrderIndex)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(Video.OrderIndex),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxVideosPerSection - 1)));

            typeBuilder.Property(v => v.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(v => v.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });

        builder.Entity<Video>()
            .HasOne<VideoFile>(video => video.VideoFile)
            .WithOne(videoFile => videoFile.Video)
            .HasForeignKey<VideoFile>(videoFile => videoFile.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Video>()
            .HasMany<VideoProgress.VideoProgress>(video => video.VideoProgresses)
            .WithOne(videoProgress => videoProgress.Video)
            .HasForeignKey(videoProgress => videoProgress.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
