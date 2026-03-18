using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Videos.VideoProgress;

/// <summary>
/// Represents the <see cref="VideoProgressConfiguration"/> type.
/// </summary>
public static class VideoProgressConfiguration
{
    /// <summary>
    /// Configures the target component.
    /// </summary>
    /// <param name="builder">The <paramref name="builder"/> argument.</param>
    public static void ConfigureVideoProgress(this ModelBuilder builder)
    {
        builder.Entity<VideoProgress>()
            .ToTable(nameof(VideoProgress), $"{nameof(Course).ToLowerInvariant()}s")
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
                    $"CK_{nameof(VideoProgress)}_{nameof(VideoProgress.PositionSeconds)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(VideoProgress.PositionSeconds), 0, EntityConstants.MaxVideoDurationSeconds)));

            typeBuilder.Property(vp => vp.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });
    }
}
