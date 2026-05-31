using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.DAL.Entities.Courses.Courses;

namespace Studi.XUnitTests.TestData;

public static class CourseTestData
{
    #region Entities

    public static Course GetCourse(
        Guid? courseId = null,
        Guid? ownerId = null,
        int? courseNumber = null,
        int sectionsCount = 2,
        int videosCount = 2,
        string title = "Title of a Course",
        string description = "Description for a Course")
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
            OwnerUserId = ownerUserId,
            OwnerUser = AppUserTestData.GetUser(ownerUserId),
            Sections = Enumerable.Range(1, sectionsCount).Select(i => SectionTestData.GetSection(i, videosCount, id)).ToList()
        };
    }

    #endregion

    #region DTOs

    public static CourseResponseDto GetCourseResponseDto(
        string title = "Title of a Course",
        string description = "Description of a Course",
        float totalDurationHours = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CourseResponseDto()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            TotalDurationHours = totalDurationHours,
            WatchingUsersCount = watchingUsersCount
        };
    }

    public static CourseResponseDto GetCourseResponseDto(
        Course course,
        float totalDurationHours = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CourseResponseDto()
        {
            Id = Guid.NewGuid(),
            Title = course.Title,
            Description = course.Description,
            SectionsCount = course.SectionsCount,
            TotalDurationHours = totalDurationHours,
            WatchingUsersCount = watchingUsersCount,
        };
    }

    public static CoursePreviewResponseDto GetCoursePreviewResponseDto(
        Course course,
        float totalDurationHours = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CoursePreviewResponseDto()
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            SectionsCount = course.SectionsCount,
            TotalDurationHours = totalDurationHours,
            WatchingUsersCount = watchingUsersCount,
        };
    }

    public static CoursePreviewShortResponseDto GetCoursePreviewShortResponseDto(
        Course course,
        int index = 0,
        float totalDurationHours = 999.9f,
        int watchingUsersCount = 123)
    {
        return new CoursePreviewShortResponseDto()
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            VideosCount = course.Sections.Sum(s => s.VideosCount),
            TotalDurationHours = totalDurationHours + index,
            WatchingUsersCount = watchingUsersCount + index,
        };
    }

    #endregion
}
