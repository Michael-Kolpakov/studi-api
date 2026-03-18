using Teachio.DAL.Utils.Constants;

namespace Teachio.DAL.Utils.Helpers;

/// <summary>
/// Represents the <see cref="Constraint"/> type.
/// </summary>
public class Constraint
{
    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="columnName">The <paramref name="columnName"/> argument.</param>
    /// <param name="rule">The <paramref name="rule"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
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

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="columnName">The <paramref name="columnName"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static string CreateSqlNonNegativeCheck(string columnName)
    {
        return $"[{columnName}] >= 0";
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="columnName">The <paramref name="columnName"/> argument.</param>
    /// <param name="minValue">The <paramref name="minValue"/> argument.</param>
    /// <param name="maxValue">The <paramref name="maxValue"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static string CreateSqlRangeCheck(string columnName, int minValue, int maxValue)
    {
        return $"[{columnName}] >= {minValue} AND [{columnName}] <= {maxValue}";
    }

    private static string ConvertRegexToSqlLike(string classicRegexPattern)
    {
        if (!classicRegexPattern.StartsWith("^[", StringComparison.Ordinal)
            || !classicRegexPattern.EndsWith("]+$", StringComparison.Ordinal))
        {
            throw new ArgumentException("Unsupported regex pattern format.", nameof(classicRegexPattern));
        }

        var sqlRegexPattern = classicRegexPattern[2..^3]
            .Replace("'", "''");

        return $"%[^{sqlRegexPattern}]%";
    }
}

/// <summary>
/// Defines possible values for <see cref="ValidationRule"/>.
/// </summary>
public enum ValidationRule
{
    Title,
    Description,
    Name,
    MediaName,
    ProcessingError
}

/// <summary>
/// Defines possible values for <see cref="CheckConstraintType"/>.
/// </summary>
public enum CheckConstraintType
{
    Regex,
    NonNegative,
    Range
}
