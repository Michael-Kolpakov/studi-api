using Teachio.DAL.Entities.Courses.Videos.VideoProgress;

namespace Teachio.XUnitTests.TestData;

/// <summary>
/// Represents the <see cref="VideoProgressTestData"/> type.
/// </summary>
public static class VideoProgressTestData
{
    #region Entities

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="isCompleted">A value indicating whether <paramref name="isCompleted"/> is enabled.</param>
    /// <param name="positionSeconds">The <paramref name="positionSeconds"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static VideoProgress GetVideoProgress(
        bool isCompleted = false,
        int positionSeconds = 560)
    {
        return new VideoProgress()
        {
            Id = Guid.NewGuid(),
            IsCompleted = isCompleted,
            PositionSeconds = positionSeconds
        };
    }

    #endregion
}
