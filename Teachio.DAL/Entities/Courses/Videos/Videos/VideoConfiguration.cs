using Microsoft.EntityFrameworkCore;

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
                .HasMaxLength(60);

            typeBuilder.Property(v => v.VideoRelativePath)
                .IsRequired()
                .HasMaxLength(500);

            typeBuilder.Property(v => v.ContentType)
                .HasMaxLength(100);

            typeBuilder.Property(v => v.SectionId)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_OrderIndex_NonNegative",
                    "[OrderIndex] >= 0"));

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_DurationSeconds_NonNegative",
                    "[DurationSeconds] >= 0"));

            typeBuilder.Property(v => v.ThumbnailName)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.Property(v => v.ThumbnailRelativePath)
                .IsRequired()
                .HasMaxLength(500);

            typeBuilder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>();

            typeBuilder.Property(v => v.ProcessingError)
                .HasMaxLength(100);

            typeBuilder.Property(course => course.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.UpdatedAt)
                .IsRequired();

            typeBuilder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();
        });

        builder.Entity<Video>()
            .HasOne<VideoProgress.VideoProgress>(video => video.VideoProgress)
            .WithOne(videoProgress => videoProgress.Video)
            .HasForeignKey<VideoProgress.VideoProgress>(videoProgress => videoProgress.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
