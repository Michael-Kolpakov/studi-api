using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Shared;

namespace Teachio.DAL.Entities.Courses.Videos.Videos;

public static class VideoConfiguration
{
    public static void ConfigureVideos(this ModelBuilder builder)
    {
        builder.Entity<Video>()
            .ToTable("Videos", "courses")
            .HasKey(v => v.Id);

        builder.Entity<Video>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.Property(v => v.VideoName)
                .IsRequired()
                .HasMaxLength(110);

            typeBuilder.Property(v => v.ContentType)
                .HasMaxLength(100);

            typeBuilder.Property(v => v.SectionId)
                .IsRequired();

            typeBuilder.Property(v => v.OrderIndex)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_OrderIndex_NonNegative",
                    "[OrderIndex] >= 0"));

            typeBuilder.Property(v => v.DurationSeconds)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_DurationSeconds_Range",
                    $"[DurationSeconds] >= 0 AND [DurationSeconds] <= {EntityConstants.MaxVideoDurationSeconds}"));

            typeBuilder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>();

            typeBuilder.Property(v => v.ProcessingError)
                .HasMaxLength(100);

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
