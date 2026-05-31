using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Studi.BLL.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class AllowedContentTypesAttribute : LocalizedValidationAttribute
{
    private readonly Type _sourceType;
    private readonly string _memberName;
    private string[]? _allowedContentTypes;

    public AllowedContentTypesAttribute(Type sourceType, string memberName)
    {
        _sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        _memberName = string.IsNullOrWhiteSpace(memberName)
            ? throw new ArgumentException("Member name must be provided.", nameof(memberName))
            : memberName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var allowedContentTypes = GetAllowedContentTypes();
        var allowedDisplayValue = BuildAllowedListDisplayValue(allowedContentTypes);

        if (value is not IFormFile file)
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, allowedDisplayValue);
        }

        if (string.IsNullOrWhiteSpace(file.ContentType)
            || !allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, allowedDisplayValue);
        }

        return ValidationResult.Success;
    }

    private string[] GetAllowedContentTypes()
    {
        if (_allowedContentTypes is not null)
        {
            return _allowedContentTypes;
        }

        var field = _sourceType.GetField(
            _memberName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (field?.GetValue(null) is IEnumerable<string> fieldValues)
        {
            _allowedContentTypes = fieldValues.ToArray();

            return _allowedContentTypes;
        }

        var property = _sourceType.GetProperty(
            _memberName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (property?.GetValue(null) is IEnumerable<string> propertyValues)
        {
            _allowedContentTypes = propertyValues.ToArray();

            return _allowedContentTypes;
        }

        throw new InvalidOperationException(
            $"Member '{_memberName}' on '{_sourceType.FullName}' does not expose allowed content types.");
    }

    private static string BuildAllowedListDisplayValue(string[] allowedContentTypes)
    {
        return allowedContentTypes.Length == 0
            ? "none"
            : string.Join(", ", allowedContentTypes);
    }
}
