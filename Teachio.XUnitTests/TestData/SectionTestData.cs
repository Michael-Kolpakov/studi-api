using Teachio.DAL.Entities.Courses.Sections;

namespace Teachio.XUnitTests.TestData;

/// <summary>
/// Represents the <see cref="SectionTestData"/> type.
/// </summary>
public static class SectionTestData
{
    #region Entities

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="sectionNumber">The <paramref name="sectionNumber"/> argument.</param>
    /// <param name="videosCount">The <paramref name="videosCount"/> argument.</param>
    /// <param name="courseId">The identifier of <paramref name="courseId"/>.</param>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
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
