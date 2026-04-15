using Microsoft.EntityFrameworkCore;

namespace Teachio.DAL.Entities.Users;

public static class AppUserConfiguration
{
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
                .IsRequired();

            typeBuilder.Property(au => au.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
        });
    }
}
