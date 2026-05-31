using System.Reflection;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Users.Users;
using Studi.XIntegrationTests.ControllerTests;
using Studi.XIntegrationTests.Utils.Extractors;
using Xunit.Sdk;

namespace Studi.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractDeleteTestCourseAttribute : BeforeAfterTestAttribute
{
    public static Course Course { get; private set; } = null!;

    public override void Before(MethodInfo methodUnderTest)
    {
        const string courseTitle = "Course for Delete Test";

        var courseId = Guid.NewGuid();
        var appUserId = Guid.NewGuid();

        Course = CourseExtractor.Extract(courseId, appUserId, courseTitle);
    }

    public override void After(MethodInfo methodUnderTest)
    {
        var sqlDbHelper = BaseControllerTests.GetSqlDbHelper();
        var appUser = sqlDbHelper.GetItemWithId<AppUser>(Course.OwnerUserId);

        if (appUser is not null)
        {
            sqlDbHelper.DeleteItem(appUser);
            sqlDbHelper.SaveChanges();
        }
    }
}
