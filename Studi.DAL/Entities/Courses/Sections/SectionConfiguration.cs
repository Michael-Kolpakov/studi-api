using Microsoft.EntityFrameworkCore;
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Helpers;

namespace Studi.DAL.Entities.Courses.Sections;

public static class SectionConfiguration
{
    public static void ConfigureSections(this ModelBuilder builder)
    {
        builder.Entity<Section>()
            .ToTable($"{nameof(Section)}s", DatabaseConstants.CoursesSchema)
            .HasKey(s => s.Id);

        builder.Entity<Section>()
            .HasIndex(s => new { s.CourseId, s.SectionName })
            .IsUnique();

        builder.Entity<Section>()
            .HasIndex(s => new { s.CourseId, s.OrderIndex })
            .IsUnique();

        builder.Entity<Section>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxSectionTitleLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.Title)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Section.Title), ValidationRule.Title)));

            typeBuilder.Property(s => s.SectionName)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxSectionNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.SectionName)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(Section.SectionName), ValidationRule.EntityName)));

            typeBuilder.Property(s => s.OrderIndex)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.OrderIndex)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(Section.OrderIndex),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxSectionsPerCourse - 1)));

            typeBuilder.Property(s => s.VideosCount)
                .IsRequired()
                .HasDefaultValue(EntityConstants.MinNonNegativeValue);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.VideosCount)}_{nameof(CheckConstraintType.Range)}",
                    ConstraintHelper.CreateSqlRangeCheck(
                        nameof(Section.VideosCount),
                        EntityConstants.MinNonNegativeValue,
                        EntityConstants.MaxVideosPerSection)));

            typeBuilder.Property(s => s.CourseId)
                .IsRequired();

            typeBuilder.Property(s => s.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(s => s.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });

        builder.Entity<Section>()
            .HasMany<Video>(section => section.Videos)
            .WithOne(video => video.Section)
            .HasForeignKey(video => video.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
