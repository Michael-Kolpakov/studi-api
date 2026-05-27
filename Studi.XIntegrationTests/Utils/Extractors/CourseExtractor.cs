using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Users.Users;

namespace Studi.XIntegrationTests.Utils.Extractors;

public static class CourseExtractor
{
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

    public static void Remove(Course course)
    {
        BaseExtractor.RemoveById<Course>(course.Id);
        BaseExtractor.RemoveById<AppUser>(course.OwnerUserId);
    }
}
