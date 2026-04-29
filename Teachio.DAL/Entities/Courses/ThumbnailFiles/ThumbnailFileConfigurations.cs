using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.ThumbnailFiles;

public static class ThumbnailFileConfigurations
{
    public static void ConfigureThumbnailFiles(this ModelBuilder builder)
    {
        builder.Entity<ThumbnailFile>()
            .ToTable($"{nameof(ThumbnailFile)}s", DatabaseConstants.CoursesSchema)
            .HasKey(v => v.Id);

        builder.Entity<ThumbnailFile>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.ThumbnailName)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxThumbnailFileNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(ThumbnailFile)}_{nameof(ThumbnailFile.ThumbnailName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(ThumbnailFile.ThumbnailName), ValidationRule.MediaName)));

            typeBuilder.Property(v => v.ContentType)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxMediaContentTypeLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(ThumbnailFile)}_{nameof(ThumbnailFile.ContentType)}_{nameof(CheckConstraintType.AllowedValues)}",
                    Constraint.CreateSqlAllowedValuesCheck(nameof(ThumbnailFile.ContentType), EntityConstants.AllowedThumbnailContentTypes)));

            typeBuilder.Property(v => v.Resolution)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxThumbnailResolutionLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(ThumbnailFile)}_{nameof(ThumbnailFile.Resolution)}_{nameof(CheckConstraintType.AspectRatioRange)}",
                    Constraint.CreateSqlAspectRatioRangeCheck(
                        nameof(ThumbnailFile.Resolution),
                        EntityConstants.MinThumbnailWidth,
                        EntityConstants.MaxThumbnailWidth,
                        EntityConstants.MinThumbnailHeight,
                        EntityConstants.MaxThumbnailHeight,
                        widthRatio: EntityConstants.ThumbnailAspectRatioWidth,
                        heightRatio: EntityConstants.ThumbnailAspectRatioHeight)));

            typeBuilder.Property(vp => vp.CourseId)
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
