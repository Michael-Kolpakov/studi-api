using Teachio.DAL.Utils.Constants;

namespace Teachio.DAL.Utils.Helpers;

public class Constraint
{
    public static string CreateSqlRegexCheck(string columnName, ValidationRule rule)
    {
        string sqlRegexPattern = rule switch
        {
            ValidationRule.Title => ConvertRegexToSqlLike(EntityConstants.TitleRegexPattern),
            ValidationRule.Description => ConvertRegexToSqlLike(EntityConstants.DescriptionRegexPattern),
            ValidationRule.Name => ConvertRegexToSqlLike(EntityConstants.NameRegexPattern),
            ValidationRule.MediaName => ConvertRegexToSqlLike(EntityConstants.MediaNameRegexPattern),
            ValidationRule.ProcessingError => ConvertRegexToSqlLike(EntityConstants.ProcessingErrorRegexPattern),
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
        };

        return $"[{columnName}] NOT LIKE '{sqlRegexPattern}'";
    }

    public static string CreateSqlNonNegativeCheck(string columnName)
    {
        return $"[{columnName}] >= 0";
    }

    public static string CreateSqlRangeCheck(string columnName, int minValue, int maxValue)
    {
        return $"[{columnName}] >= {minValue} AND [{columnName}] <= {maxValue}";
    }

    private static string ConvertRegexToSqlLike(string classicRegexPattern)
    {
        if (!classicRegexPattern.StartsWith("^[") || !classicRegexPattern.EndsWith("]+$"))
        {
            throw new ArgumentException("Unsupported regex pattern format.", nameof(classicRegexPattern));
        }

        var sqlRegexPattern = classicRegexPattern
            .Substring(2, classicRegexPattern.Length - 5)
            .Replace("'", "''");

        return $"%[^{sqlRegexPattern}]%";
    }
}

public enum ValidationRule
{
    Title,
    Description,
    Name,
    MediaName,
    ProcessingError
}

public enum CheckConstraintType
{
    Regex,
    NonNegative,
    Range
}
