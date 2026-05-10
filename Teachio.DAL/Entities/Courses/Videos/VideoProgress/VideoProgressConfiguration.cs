using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Videos.VideoProgress;

public static class VideoProgressConfiguration
{
    public static void ConfigureVideoProgress(this ModelBuilder builder)
    {
        builder.Entity<VideoProgress>()
            .ToTable(nameof(VideoProgress), DatabaseConstants.CoursesSchema)
            .HasKey(vp => vp.Id);

        builder.Entity<VideoProgress>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(vp => vp.VideoId)
                .IsRequired();

            typeBuilder.Property(vp => vp.IsCompleted)
                .IsRequired()
                .HasDefaultValue(EntityConstants.DefaultVideoProgressIsCompleted);

            typeBuilder.Property(vp => vp.PositionSeconds)
                .IsRequired()
                .HasDefaultValue(EntityConstants.MinNonNegativeValue);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(VideoProgress)}_{nameof(VideoProgress.PositionSeconds)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(VideoProgress.PositionSeconds),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxVideoDurationSeconds)));

            typeBuilder.Property(vp => vp.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });
    }
}
