using Microsoft.EntityFrameworkCore;

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

            typeBuilder.Property(vp => vp.PositionSeconds)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_PositionSeconds_NonNegative",
                    "[PositionSeconds] >= 0"));

            typeBuilder.Property(vp => vp.UpdatedAt)
                .IsRequired();
        });
    }
}
