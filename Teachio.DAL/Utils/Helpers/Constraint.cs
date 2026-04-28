using Teachio.DAL.Utils.Constants;

namespace Teachio.DAL.Utils.Helpers;

public static class Constraint
{
    public static string CreateSqlRegexCheck(string columnName, ValidationRule rule)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);

        string sqlRegexPattern = rule switch
        {
            ValidationRule.Title => ConvertRegexToSqlLike(EntityConstants.TitleRegexPattern),
            ValidationRule.Description => ConvertRegexToSqlLike(EntityConstants.DescriptionRegexPattern),
            ValidationRule.Name => ConvertRegexToSqlLike(EntityConstants.NameRegexPattern),
            ValidationRule.MediaName => ConvertRegexToSqlLike(EntityConstants.MediaNameRegexPattern),
            ValidationRule.ProcessingError => ConvertRegexToSqlLike(EntityConstants.ProcessingErrorRegexPattern),
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
        };

        return $"{sqlColumnName} NOT LIKE '{sqlRegexPattern}'";
    }

    public static string CreateSqlNonNegativeCheck(string columnName)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);

        return $"{sqlColumnName} >= 0";
    }

    public static string CreateSqlRangeCheck(string columnName, int minValue, int maxValue)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);

        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minValue),
                minValue,
                "Minimum value cannot be greater than maximum value.");
        }

        return $"{sqlColumnName} >= {minValue} AND {sqlColumnName} <= {maxValue}";
    }

    public static string CreateSqlAspectRatioRangeCheck(
        string columnName,
        int minWidth,
        int maxWidth,
        int minHeight,
        int maxHeight,
        int widthRatio,
        int heightRatio)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);

        if (minWidth > maxWidth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minWidth),
                minWidth,
                "Minimum width cannot be greater than maximum width.");
        }

        if (minHeight > maxHeight)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minHeight),
                minHeight,
                "Minimum height cannot be greater than maximum height.");
        }

        if (widthRatio <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(widthRatio),
                widthRatio,
                "Width ratio must be positive.");
        }

        if (heightRatio <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(heightRatio),
                heightRatio,
                "Height ratio must be positive.");
        }

        var indexExpression = $"CHARINDEX('x', {sqlColumnName})";
        var widthExpression = $"TRY_CONVERT(int, LEFT({sqlColumnName}, {indexExpression} - 1))";
        var heightExpression = $"TRY_CONVERT(int, SUBSTRING({sqlColumnName}, {indexExpression} + 1, LEN({sqlColumnName})))";

        return $"{indexExpression} > 1 AND "
            + $"{indexExpression} < LEN({sqlColumnName}) AND "
            + $"CHARINDEX('x', {sqlColumnName}, {indexExpression} + 1) = 0 AND "
            + $"{widthExpression} IS NOT NULL AND {heightExpression} IS NOT NULL AND "
            + $"{widthExpression} >= {minWidth} AND {widthExpression} <= {maxWidth} AND "
            + $"{heightExpression} >= {minHeight} AND {heightExpression} <= {maxHeight} AND "
            + $"{widthExpression} * {heightRatio} = {heightExpression} * {widthRatio}";
    }

    public static string CreateSqlAllowedValuesCheck(string columnName, IEnumerable<string> allowedValues)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);
        ArgumentNullException.ThrowIfNull(allowedValues);

        var escapedValues = new List<string>();

        foreach (var allowedValue in allowedValues)
        {
            if (string.IsNullOrWhiteSpace(allowedValue))
            {
                throw new ArgumentException(
                    "Allowed values cannot contain null or whitespace entries.",
                    nameof(allowedValues));
            }

            escapedValues.Add($"'{allowedValue.Replace("'", "''")}'");
        }

        if (escapedValues.Count == 0)
        {
            throw new ArgumentException("At least one allowed value must be provided.", nameof(allowedValues));
        }

        return $"{sqlColumnName} IN ({string.Join(", ", escapedValues)})";
    }

    public static string CreateSqlAllowedValuesCheck(string columnName, IEnumerable<int> allowedValues)
    {
        var sqlColumnName = EscapeSqlColumnName(columnName);
        ArgumentNullException.ThrowIfNull(allowedValues);

        var normalizedValues = allowedValues
            .Distinct()
            .OrderBy(value => value)
            .ToList();

        if (normalizedValues.Count == 0)
        {
            throw new ArgumentException("At least one allowed value must be provided.", nameof(allowedValues));
        }

        return $"{sqlColumnName} IN ({string.Join(", ", normalizedValues)})";
    }

    private static string EscapeSqlColumnName(string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (!string.Equals(columnName, columnName.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Column name cannot contain leading or trailing whitespace.",
                nameof(columnName));
        }

        if (columnName.Any(char.IsControl))
        {
            throw new ArgumentException("Column name cannot contain control characters.", nameof(columnName));
        }

        return $"[{columnName.Replace("]", "]]", StringComparison.Ordinal)}]";
    }

    private static string ConvertRegexToSqlLike(string classicRegexPattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(classicRegexPattern);

        if (!classicRegexPattern.StartsWith("^[", StringComparison.Ordinal)
            || !classicRegexPattern.EndsWith("]+$", StringComparison.Ordinal))
        {
            throw new ArgumentException("Unsupported regex pattern format.", nameof(classicRegexPattern));
        }

        var sqlRegexPattern = classicRegexPattern[2..^3]
            .Replace("'", "''");

        if (sqlRegexPattern.Length == 0)
        {
            throw new ArgumentException("Regex character set cannot be empty.", nameof(classicRegexPattern));
        }

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
    Range,
    AllowedValues,
    AspectRatioRange
}
