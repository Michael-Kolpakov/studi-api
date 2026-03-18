using Microsoft.EntityFrameworkCore;

namespace Teachio.DAL.Entities.Users;

/// <summary>
/// Represents the <see cref="AppUserConfiguration"/> type.
/// </summary>
public static class AppUserConfiguration
{
    /// <summary>
    /// Configures the target component.
    /// </summary>
    /// <param name="builder">The <paramref name="builder"/> argument.</param>
    public static void ConfigureAppUsers(this ModelBuilder builder)
    {
        builder.Entity<AppUser>()
            .ToTable($"{nameof(AppUser)}s", "users")
            .HasKey(au => au.Id);

        builder.Entity<AppUser>(typeBuilder =>
        {
            typeBuilder.Property(au => au.Name)
                .IsRequired()
                .HasMaxLength(20);

            typeBuilder.Property(au => au.Surname)
                .IsRequired()
                .HasMaxLength(30);

            typeBuilder.Property(au => au.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            typeBuilder.Property(au => au.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });
    }
}
