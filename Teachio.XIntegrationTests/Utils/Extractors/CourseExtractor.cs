using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;

namespace Teachio.XIntegrationTests.Utils.Extractors;

public static class CourseExtractor
{
    public static Course Extract(Guid courseId, string? title = null)
    {
        var appUserId = Guid.NewGuid();

        var testCourse = TestDataProvider.GetTestData<Course>();
        var testAppUser = AppUserExtractor.Extract(appUserId);

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
