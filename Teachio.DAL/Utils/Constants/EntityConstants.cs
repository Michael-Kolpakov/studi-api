namespace Teachio.DAL.Utils.Constants;

public static class EntityConstants
{
    #region Limits

    public const int MaxVideoDurationSeconds = 3600;

    public const long MaxVideoFileSizeBytes = 2L * 1024 * 1024 * 1024; // 2 GB

    public const int MaxVideoTitleLength = 60;

    public const int MaxCourseTitleLength = 60;

    public const int MaxCourseDescriptionLength = 1000;

    public const int MaxCourseNameLength = 60;

    public const int MaxSectionTitleLength = 60;

    public const int MaxSectionNameLength = 60;

    public const int MinVideoResolution = 480;

    public const int MaxVideoResolution = 1440;

    public const int MinThumbnailWidth = 1280;

    public const int MaxThumbnailWidth = 2560;

    public const int MinThumbnailHeight = 720;

    public const int MaxThumbnailHeight = 1440;

    public const int ThumbnailAspectRatioWidth = 16;

    public const int ThumbnailAspectRatioHeight = 9;

    public const int MaxThumbnailFileNameLength = 110;

    public const int MaxThumbnailResolutionLength = 9;

    public const int MaxMediaContentTypeLength = 20;

    public const int MaxVideoFileNameLength = 110;

    public const int MaxVideoProcessingErrorLength = 100;

    public const int MaxSectionsPerCourse = 35;

    public const int MaxVideosPerSection = 40;

    public const int MaxUserNameLength = 20;

    public const int MaxUserSurnameLength = 30;

    #endregion

    #region Defaults

    public const int MinNonNegativeValue = 0;

    public const bool DefaultVideoProgressIsCompleted = false;

    #endregion

    #region RegexPatterns

    public const string TitleRegexPattern = "^[A-Za-z0-9 ,!?-]+$";

    public const string DescriptionRegexPattern = @"^[A-Za-z0-9 ,.!?%$#""':&()+=*/-–]+$";

    public const string NameRegexPattern = "^[A-Za-z0-9,!?-]+$";

    public const string MediaNameRegexPattern = "^[A-Za-z0-9.,!?-]+$";

    public const string ProcessingErrorRegexPattern = "^[A-Za-z0-9 .,]+$";

    #endregion

    #region AllowedValues

    public static readonly string[] AllowedThumbnailContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    public static readonly string[] AllowedVideoContentTypes =
    [
        "video/mp4",
        "video/quicktime"
    ];

    public static readonly int[] AllowedVideoResolutions =
    [
        480,
        720,
        1080,
        1440
    ];

    #endregion
}
