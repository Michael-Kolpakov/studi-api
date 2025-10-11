using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Videos.Videos;

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

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Video_OrderIndex_NonNegative",
                    "[OrderIndex] >= 0"));

            typeBuilder.Property(s => s.CourseId)
                .IsRequired();

            typeBuilder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(s => s.UpdatedAt)
                .IsRequired();
        });

        builder.Entity<Section>()
            .HasMany<Video>(section => section.Videos)
            .WithOne(video => video.Section)
            .HasForeignKey(video => video.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
