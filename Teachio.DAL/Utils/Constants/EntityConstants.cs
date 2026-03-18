namespace Teachio.DAL.Utils.Constants;

/// <summary>
/// Represents the <see cref="EntityConstants"/> type.
/// </summary>
public static class EntityConstants
{
    public const int MaxVideoDurationSeconds = 3600;

    public const int MaxSectionsPerCourse = 35;

    public const int MaxVideosPerSection = 40;

    public const string TitleRegexPattern = "^[A-Za-z0-9 ,!?-]+$";

    public const string DescriptionRegexPattern = @"^[A-Za-z0-9 ,.!?%$#""':&()+=*/-–]+$";

    public const string NameRegexPattern = "^[A-Za-z0-9,!?-]+$";

    public const string MediaNameRegexPattern = "^[A-Za-z0-9.,!?-]+$";

    public const string ProcessingErrorRegexPattern = "^[A-Za-z0-9 .,]+$";
}
