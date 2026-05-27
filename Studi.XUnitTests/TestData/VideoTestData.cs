using Studi.BLL.DTOs.Courses.Videos.Videos.Response;
using Studi.DAL.Entities.Courses.Videos.VideoProgress;
using Studi.DAL.Entities.Courses.Videos.Videos;

namespace Studi.XUnitTests.TestData;

public static class VideoTestData
{
    #region Entities

    public static Video GetVideo(
        int videoNumber,
        Guid sectionId,
        string title = "Title of a Video")
    {
        var actualTitle = string.Join(" ", title, videoNumber);

        return new Video()
        {
            Id = Guid.NewGuid(),
            Title = actualTitle,
            SectionId = sectionId,
            OrderIndex = videoNumber,
            VideoProgresses = new List<VideoProgress>
            {
                VideoProgressTestData.GetVideoProgress()
            }
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
            SectionId = video.SectionId,
            OrderIndex = video.OrderIndex,
        };
    }

    #endregion
}
