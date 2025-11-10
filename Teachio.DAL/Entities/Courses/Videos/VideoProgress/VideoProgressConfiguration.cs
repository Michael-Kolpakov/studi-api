using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Shared;

namespace Teachio.DAL.Entities.Courses.Videos.VideoProgress;

public static class VideoProgressConfiguration
{
    public static void ConfigureVideoProgress(this ModelBuilder builder)
    {
        builder.Entity<VideoProgress>()
            .ToTable("VideoProgress", "courses")
            .HasKey(vp => vp.Id);

        builder.Entity<VideoProgress>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(vp => vp.VideoId)
                .IsRequired();

            typeBuilder.Property(vp => vp.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            typeBuilder.Property(vp => vp.PositionSeconds)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_PositionSeconds_Range",
                    $"[PositionSeconds] >= 0 AND [PositionSeconds] <= {EntityConstants.MaxVideoDurationSeconds}"));

            typeBuilder.Property(vp => vp.UpdatedAt)
                .IsRequired();
        });
    }
}
