namespace Studi.BLL.Utils.Helpers;

public static class UserFullNameHelper
{
    public static string BuildFullName(string? name, string? surname)
    {
        var safeName = name?.Trim() ?? string.Empty;
        var safeSurname = surname?.Trim() ?? string.Empty;

        if (safeName.Length == 0)
        {
            return safeSurname;
        }

        if (safeSurname.Length == 0)
        {
            return safeName;
        }

        return $"{safeName} {safeSurname}";
    }
}
