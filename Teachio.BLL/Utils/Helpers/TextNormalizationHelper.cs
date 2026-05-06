namespace Teachio.BLL.Utils.Helpers;

public static class TextNormalizationHelper
{
    public static string CapitalizeFirstLatinLetter(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value[0] is >= 'a' and <= 'z'
            ? string.Concat(char.ToUpperInvariant(value[0]), value[1..])
            : value;
    }

    public static string TrimAndCapitalizeFirstLatinLetter(string value) =>
        CapitalizeFirstLatinLetter(value.Trim());
}
