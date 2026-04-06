using Teachio.DAL.Entities.Courses.Videos.VideoProgress;

namespace Teachio.XUnitTests.TestData;

public static class VideoProgressTestData
{
    #region Entities

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
