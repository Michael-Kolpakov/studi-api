using System.Reflection;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;
using Teachio.XIntegrationTests.ControllerTests;
using Teachio.XIntegrationTests.Utils.Extractors;
using Xunit.Sdk;

namespace Teachio.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractDeleteTestCourseAttribute : BeforeAfterTestAttribute
{
    public static Course Course { get; private set; } = null!;

    public override void Before(MethodInfo methodUnderTest)
    {
        const string CourseTitle = "Course for Delete Test";

        var courseId = Guid.NewGuid();

        Course = CourseExtractor.Extract(courseId, CourseTitle);
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
