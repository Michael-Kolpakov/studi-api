using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Shared;

namespace Teachio.DAL.Entities.Courses.Sections;

public static class SectionConfiguration
{
    public static void ConfigureSections(this ModelBuilder builder)
    {
        builder.Entity<Section>()
            .ToTable("Sections", "courses")
            .HasKey(s => s.Id);

        builder.Entity<Section>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(60);

            typeBuilder.Property(s => s.OrderIndex)
                .IsRequired();

            typeBuilder.Property(s => s.VideosCount)
                .IsRequired()
                .HasDefaultValue(0);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Section_VideosCount_Max",
                    $"[VideosCount] >= 0 AND [VideosCount] <= {EntityConstants.MaxVideosPerSection}"));
            
            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Section_OrderIndex_NonNegative",
                    "[OrderIndex] >= 0"));

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
