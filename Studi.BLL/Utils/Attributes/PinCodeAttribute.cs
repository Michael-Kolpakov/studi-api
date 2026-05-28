using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Studi.BLL.Models.Auth;
using Studi.BLL.Services.Interfaces;

namespace Studi.BLL.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PinCodeAttribute : LocalizedValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var expectedLength = ResolveExpectedLength(validationContext);

        if (value is not string pinCode || string.IsNullOrWhiteSpace(pinCode))
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, expectedLength);
        }

        var pinCodeService = validationContext.GetService(typeof(IPinCodeService)) as IPinCodeService;
        if (pinCodeService is not null)
        {
            if (!pinCodeService.IsValidPin(pinCode))
            {
                return BuildValidationResult(validationContext, validationContext.DisplayName, expectedLength);
            }

            return ValidationResult.Success;
        }

        if (pinCode.Length != expectedLength || !pinCode.All(char.IsDigit))
        {
            return BuildValidationResult(validationContext, validationContext.DisplayName, expectedLength);
        }

        return ValidationResult.Success;
    }

    private static int ResolveExpectedLength(ValidationContext validationContext)
    {
        var options = (validationContext.GetService(typeof(IOptions<EmailVerificationOptions>)) as IOptions<EmailVerificationOptions>)?.Value;
        var expectedLength = options?.PinLength ?? new EmailVerificationOptions().PinLength;

        return expectedLength > 0 ? expectedLength : new EmailVerificationOptions().PinLength;
    }
}
