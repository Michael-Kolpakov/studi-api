using Microsoft.EntityFrameworkCore;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Helpers;

namespace Studi.DAL.Entities.Users.PendingRegistrations;

public static class PendingRegistrationConfiguration
{
    public static void ConfigurePendingRegistrations(this ModelBuilder builder)
    {
        builder.Entity<PendingRegistration>()
            .ToTable($"{nameof(PendingRegistration)}s", DatabaseConstants.UsersSchema)
            .HasKey(pr => pr.Id);

        builder.Entity<PendingRegistration>(typeBuilder =>
        {
            typeBuilder.Property(pr => pr.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(pr => pr.Email)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserEmailLength);

            typeBuilder.Property(pr => pr.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserEmailLength);

            typeBuilder.HasIndex(pr => pr.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName($"IX_{nameof(PendingRegistration)}_{nameof(PendingRegistration.NormalizedEmail)}");

            typeBuilder.Property(pr => pr.Name)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(PendingRegistration)}_{nameof(PendingRegistration.Name)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(PendingRegistration.Name), ValidationRule.UserName)));

            typeBuilder.Property(pr => pr.Surname)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxUserSurnameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(PendingRegistration)}_{nameof(PendingRegistration.Surname)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(PendingRegistration.Surname), ValidationRule.UserName)));

            typeBuilder.Property(pr => pr.PasswordHash)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxPasswordHashLength);

            typeBuilder.Property(pr => pr.PinHash)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxVerificationPinHashLength);

            typeBuilder.Property(pr => pr.PinExpiresAtUtc)
                .IsRequired();

            typeBuilder.Property(pr => pr.PinSentAtUtc)
                .IsRequired();

            typeBuilder.Property(pr => pr.CreatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();

            typeBuilder.Property(pr => pr.UpdatedAt)
                .HasDefaultValueSql(DatabaseConstants.UtcNowSql)
                .IsRequired();
        });
    }
}
