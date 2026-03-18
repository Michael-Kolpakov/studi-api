using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;

namespace Teachio.XIntegrationTests.Utils.Extractors;

/// <summary>
/// Represents the <see cref="CourseExtractor"/> type.
/// </summary>
public static class CourseExtractor
{
    /// <summary>
    /// Performs the <see cref="Extract"/> operation.
    /// </summary>
    /// <param name="courseId">The identifier of <paramref name="courseId"/>.</param>
    /// <param name="userId">The identifier of <paramref name="userId"/>.</param>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static Course Extract(Guid courseId, Guid userId, string? title = null)
    {
        var testCourse = TestDataProvider.GetTestData<Course>();
        var testAppUser = AppUserExtractor.Extract(userId);

        testCourse.Id = courseId;
        testCourse.OwnerUserId = testAppUser.Id;
        testCourse.OwnerUser = testAppUser;

        if (title is not null)
        {
            testCourse.Title = title;
        }

        return BaseExtractor.Extract(testCourse, course => course.Id == courseId);
    }

    /// <summary>
    /// Deletes the target entity.
    /// </summary>
    /// <param name="course">The <paramref name="course"/> argument.</param>
    public static void Remove(Course course)
    {
        BaseExtractor.RemoveById<Course>(course.Id);
        BaseExtractor.RemoveById<AppUser>(course.OwnerUserId);
    }
}
