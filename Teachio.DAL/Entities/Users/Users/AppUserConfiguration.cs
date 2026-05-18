using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Users.AvatarFiles;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Entities.Users.Users;

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

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(AppUser)}_{nameof(AppUser.Name)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(AppUser.Name), ValidationRule.UserName)));

            typeBuilder.Property(au => au.Surname)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserSurnameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(AppUser)}_{nameof(AppUser.Surname)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(AppUser.Surname), ValidationRule.UserName)));

            typeBuilder.Property(au => au.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(au => au.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(au => au.RefreshTokenHash)
                .HasMaxLength(EntityConstants.MaxRefreshTokenHashLength);

            typeBuilder.Property(au => au.RefreshTokenCreatedAt);

            typeBuilder.Property(au => au.RefreshTokenExpiresAt);

            typeBuilder.Property(au => au.RefreshTokenRevokedAt);

            typeBuilder.HasIndex(au => au.RefreshTokenHash)
                .HasDatabaseName($"IX_{nameof(AppUser)}_{nameof(AppUser.RefreshTokenHash)}");
        });

        builder.Entity<AppUser>()
            .HasOne<AvatarFile>(appUser => appUser.AvatarFile)
            .WithOne(avatarFile => avatarFile.AppUser)
            .HasForeignKey<AvatarFile>(avatarFile => avatarFile.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
