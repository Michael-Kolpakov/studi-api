using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Courses.Sections;

public static class SectionConfiguration
{
    public static void ConfigureSections(this ModelBuilder builder)
    {
        builder.Entity<Section>()
            .ToTable($"{nameof(Section)}s", $"{nameof(Course).ToLowerInvariant()}s")
            .HasKey(s => s.Id);

        builder.Entity<Section>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.Title)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Section.Title), ValidationRule.Title)));

            typeBuilder.Property(s => s.SectionName)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.SectionName)}_{nameof(CheckConstraintType.Regex)}",
                    Constraint.CreateSqlRegexCheck(nameof(Section.SectionName), ValidationRule.Name)));

            typeBuilder.Property(s => s.OrderIndex)
                .IsRequired();

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.OrderIndex)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(Section.OrderIndex), 0, EntityConstants.MaxSectionsPerCourse - 1)));

            typeBuilder.Property(s => s.VideosCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(Section)}_{nameof(Section.VideosCount)}_{nameof(CheckConstraintType.Range)}",
                    Constraint.CreateSqlRangeCheck(nameof(Section.VideosCount), 0, EntityConstants.MaxVideosPerSection)));

            typeBuilder.Property(s => s.CourseId)
                .IsRequired();

            typeBuilder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });

        builder.Entity<Section>()
            .HasMany<Video>(section => section.Videos)
            .WithOne(video => video.Section)
            .HasForeignKey(video => video.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
