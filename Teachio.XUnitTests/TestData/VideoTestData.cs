using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.XUnitTests.TestData;

/// <summary>
/// Represents the <see cref="VideoTestData"/> type.
/// </summary>
public static class VideoTestData
{
    #region Entities

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="videoNumber">The <paramref name="videoNumber"/> argument.</param>
    /// <param name="sectionId">The identifier of <paramref name="sectionId"/>.</param>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <param name="durationSeconds">The <paramref name="durationSeconds"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static Video GetVideo(
        int videoNumber,
        Guid sectionId,
        string title = "Title of a Video",
        int durationSeconds = 1260)
    {
        var actualTitle = string.Join(" ", title, videoNumber);

        return new Video()
        {
            Id = Guid.NewGuid(),
            Title = actualTitle,
            VideoName = string.Join("-", actualTitle.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries), ".mp4"),
            SectionId = sectionId,
            OrderIndex = videoNumber,
            DurationSeconds = durationSeconds,
            Status = VideoStatus.Ready,
            VideoProgress = VideoProgressTestData.GetVideoProgress()
        };
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="video">The <paramref name="video"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static VideoResponseDto GetVideoResponseDto(Video video)
    {
        return new VideoResponseDto()
        {
            Id = video.Id,
            Title = video.Title,
            VideoRelativePath = video.VideoName,
            SectionId = video.SectionId,
            OrderIndex = video.OrderIndex,
            DurationSeconds = video.DurationSeconds
        };
    }

    #endregion
}
