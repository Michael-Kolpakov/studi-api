using Studi.DAL.Entities.Courses.Sections;

namespace Studi.XUnitTests.TestData;

public static class SectionTestData
{
    #region Entities

    public static Section GetSection(
        int sectionNumber,
        int videosCount,
        Guid courseId,
        string title = "Title of a Section")
    {
        var sectionId = Guid.NewGuid();
        var actualTitle = string.Join(" ", title, sectionNumber);

        return new Section()
        {
            Id = sectionId,
            Title = actualTitle,
            SectionName = string.Join("-", actualTitle.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries)),
            OrderIndex = sectionNumber,
            VideosCount = videosCount,
            CourseId = courseId,
            Videos = Enumerable.Range(1, videosCount).Select(i => VideoTestData.GetVideo(i, sectionId)).ToList()
        };
    }

    #endregion
}
