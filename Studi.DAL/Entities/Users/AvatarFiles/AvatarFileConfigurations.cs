using Microsoft.EntityFrameworkCore;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Helpers;

namespace Studi.DAL.Entities.Users.AvatarFiles;

public static class AvatarFileConfigurations
{
    public static void ConfigureAvatarFiles(this ModelBuilder builder)
    {
        builder.Entity<AvatarFile>()
            .ToTable($"{nameof(AvatarFile)}s", DatabaseConstants.UsersSchema)
            .HasKey(v => v.Id);

        builder.Entity<AvatarFile>(typeBuilder =>
        {
            typeBuilder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(v => v.AvatarName)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxAvatarFileNameLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(AvatarFile)}_{nameof(AvatarFile.AvatarName)}_{nameof(CheckConstraintType.Regex)}",
                    ConstraintHelper.CreateSqlRegexCheck(nameof(AvatarFile.AvatarName), ValidationRule.MediaName)));

            typeBuilder.Property(v => v.ContentType)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxMediaContentTypeLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(AvatarFile)}_{nameof(AvatarFile.ContentType)}_{nameof(CheckConstraintType.AllowedValues)}",
                    ConstraintHelper.CreateSqlAllowedValuesCheck(nameof(AvatarFile.ContentType), EntityConstants.AllowedAvatarContentTypes)));

            typeBuilder.Property(v => v.Resolution)
                .IsRequired()
                .HasMaxLength(EntityConstants.MaxAvatarResolutionLength);

            typeBuilder.ToTable(t =>
                t.HasCheckConstraint(
                    $"CK_{nameof(AvatarFile)}_{nameof(AvatarFile.Resolution)}_{nameof(CheckConstraintType.AspectRatioRange)}",
                    ConstraintHelper.CreateSqlAspectRatioRangeCheck(
                        nameof(AvatarFile.Resolution),
                        EntityConstants.MinAvatarWidth,
                        EntityConstants.MaxAvatarWidth,
                        EntityConstants.MinAvatarHeight,
                        EntityConstants.MaxAvatarHeight,
                        widthRatio: EntityConstants.AvatarAspectRatioWidth,
                        heightRatio: EntityConstants.AvatarAspectRatioHeight)));

            typeBuilder.Property(vp => vp.AppUserId)
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
