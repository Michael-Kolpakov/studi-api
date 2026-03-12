using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.XUnitTests.TestData;

public static class VideoTestData
{
    #region Entities

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
