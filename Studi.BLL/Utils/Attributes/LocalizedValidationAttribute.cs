using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Studi.DAL.SharedResource;

namespace Studi.BLL.Utils.Attributes;

public abstract class LocalizedValidationAttribute : ValidationAttribute
{
    protected ValidationResult BuildValidationResult(ValidationContext validationContext, params object[] args)
    {
        var message = ResolveLocalizedErrorMessage(validationContext, args);

        return new ValidationResult(message);
    }

    protected string ResolveLocalizedErrorMessage(ValidationContext validationContext, params object[] args)
    {
        var messageKey = ErrorMessage ?? string.Empty;
        if (string.IsNullOrWhiteSpace(messageKey))
        {
            return "Validation failed.";
        }

        var localizer = validationContext.GetService(
            typeof(IStringLocalizer<DataAnnotationsSharedResource>)) as IStringLocalizer<DataAnnotationsSharedResource>;

        if (localizer is not null)
        {
            return localizer[messageKey, args].Value;
        }

        return string.Format(CultureInfo.CurrentCulture, messageKey, args);
    }
}
