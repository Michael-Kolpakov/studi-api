using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Studi.BLL.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotEmptyFileAttribute : LocalizedValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not IFormFile file || file.Length == 0)
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName);
        }

        return ValidationResult.Success;
    }
}
