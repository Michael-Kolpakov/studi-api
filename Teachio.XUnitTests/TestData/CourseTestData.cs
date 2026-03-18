using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.XUnitTests.TestData;

/// <summary>
/// Represents the <see cref="CourseTestData"/> type.
/// </summary>
public static class CourseTestData
{
    #region Entities

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="courseId">The identifier of <paramref name="courseId"/>.</param>
    /// <param name="ownerId">The identifier of <paramref name="ownerId"/>.</param>
    /// <param name="courseNumber">The <paramref name="courseNumber"/> argument.</param>
    /// <param name="sectionsCount">The <paramref name="sectionsCount"/> argument.</param>
    /// <param name="videosCount">The <paramref name="videosCount"/> argument.</param>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <param name="description">The <paramref name="description"/> argument.</param>
    /// <param name="thumbnailName">The <paramref name="thumbnailName"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static Course GetCourse(
        Guid? courseId = null,
        Guid? ownerId = null,
        int? courseNumber = null,
        int sectionsCount = 2,
        int videosCount = 2,
        string title = "Title of a Course",
        string description = "Description for a Course",
        string thumbnailName = "title-of-a-course.png")
    {
        var id = courseId ?? Guid.NewGuid();
        var ownerUserId = ownerId ?? Guid.NewGuid();
        var actualTitle = courseNumber.HasValue
            ? string.Join(" ", title, courseNumber.Value)
            : title;

        return new Course()
        {
            Id = id,
            Title = actualTitle,
            Description = description,
            CourseName = string.Join("-", actualTitle.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries)),
            ThumbnailName = thumbnailName,
            OwnerUserId = ownerUserId,
            OwnerUser = AppUserTestData.GetUser(ownerUserId),
            Sections = Enumerable.Range(1, sectionsCount).Select(i => SectionTestData.GetSection(i, videosCount, id)).ToList()
        };
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <param name="description">The <paramref name="description"/> argument.</param>
    /// <param name="totalDuration">The <paramref name="totalDuration"/> argument.</param>
    /// <param name="watchingUsersCount">The <paramref name="watchingUsersCount"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static CourseResponseDto GetCourseResponseDto(
        string title = "Title of a Course",
        string description = "Description of a Course",
        float totalDuration = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CourseResponseDto()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            TotalDuration = totalDuration,
            WatchingUsersCount = watchingUsersCount
        };
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="course">The <paramref name="course"/> argument.</param>
    /// <param name="totalDuration">The <paramref name="totalDuration"/> argument.</param>
    /// <param name="watchingUsersCount">The <paramref name="watchingUsersCount"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static CourseResponseDto GetCourseResponseDto(
        Course course,
        float totalDuration = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CourseResponseDto()
        {
            Id = Guid.NewGuid(),
            Title = course.Title,
            Description = course.Description,
            SectionsCount = course.SectionsCount,
            TotalDuration = totalDuration,
            WatchingUsersCount = watchingUsersCount,
        };
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="course">The <paramref name="course"/> argument.</param>
    /// <param name="totalDuration">The <paramref name="totalDuration"/> argument.</param>
    /// <param name="watchingUsersCount">The <paramref name="watchingUsersCount"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static CoursePreviewResponseDto GetCoursePreviewResponseDto(
        Course course,
        float totalDuration = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CoursePreviewResponseDto()
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            SectionsCount = course.SectionsCount,
            TotalDuration = totalDuration,
            WatchingUsersCount = watchingUsersCount,
        };
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="course">The <paramref name="course"/> argument.</param>
    /// <param name="index">The <paramref name="index"/> argument.</param>
    /// <param name="totalDuration">The <paramref name="totalDuration"/> argument.</param>
    /// <param name="watchingUsersCount">The <paramref name="watchingUsersCount"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static CoursePreviewShortResponseDto GetCoursePreviewShortResponseDto(
        Course course,
        int index = 0,
        float totalDuration = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CoursePreviewShortResponseDto()
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            VideosCount = course.Sections.Sum(s => s.VideosCount),
            TotalDuration = totalDuration + index,
            WatchingUsersCount = watchingUsersCount + index,
        };
    }

    #endregion
}
