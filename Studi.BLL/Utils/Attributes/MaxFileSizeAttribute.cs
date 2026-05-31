using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Studi.BLL.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class MaxFileSizeAttribute : LocalizedValidationAttribute
{
    private const long BytesPerMegabyte = 1024L * 1024L;
    private readonly long _maxBytes;

    public MaxFileSizeAttribute(long maxBytes)
    {
        if (maxBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Max file size must be positive.");
        }

        _maxBytes = maxBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not IFormFile file)
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, GetMaxMegabytes());
        }

        if (file.Length > _maxBytes)
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, GetMaxMegabytes());
        }

        return ValidationResult.Success;
    }

    private long GetMaxMegabytes()
    {
        var maxMegabytes = _maxBytes / BytesPerMegabyte;

        return maxMegabytes > 0 ? maxMegabytes : 1;
    }
}
