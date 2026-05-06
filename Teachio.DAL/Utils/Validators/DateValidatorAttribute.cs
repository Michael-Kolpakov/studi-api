using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Teachio.DAL.Utils.Validators;

public class DateValidatorAttribute : ValidationAttribute
{
    private DateTime MinDate { get; }

    private DateTime MaxDate { get; }

    public DateValidatorAttribute(string minDate, string maxDate)
    {
        MinDate = DateTime.Parse(minDate, CultureInfo.InvariantCulture);
        MaxDate = DateTime.Parse(maxDate, CultureInfo.InvariantCulture);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not DateTime date)
        {
            return new ValidationResult("Invalid date format.");
        }

        if (date < MinDate || date > MaxDate)
        {
            return new ValidationResult(
                $"The field {validationContext.DisplayName} must be between {MinDate:d} and {MaxDate:d}.");
        }

        return ValidationResult.Success;
    }
}
