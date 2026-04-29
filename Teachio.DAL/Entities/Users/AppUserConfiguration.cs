using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Utils.Constants;

namespace Teachio.DAL.Entities.Users;

public static class AppUserConfiguration
{
    public static void ConfigureAppUsers(this ModelBuilder builder)
    {
        builder.Entity<AppUser>()
            .ToTable($"{nameof(AppUser)}s", DatabaseConstants.UsersSchema)
            .HasKey(au => au.Id);

        builder.Entity<AppUser>(typeBuilder =>
        {
            typeBuilder.Property(au => au.Name)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserNameLength);

            typeBuilder.Property(au => au.Surname)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserSurnameLength);

            typeBuilder.Property(au => au.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(au => au.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });
    }
}
