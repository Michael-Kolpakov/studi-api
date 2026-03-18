using System.Reflection;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;
using Teachio.XIntegrationTests.ControllerTests;
using Teachio.XIntegrationTests.Utils.Extractors;
using Xunit.Sdk;

namespace Teachio.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

/// <summary>
/// Represents the <see cref="ExtractDeleteTestCourseAttribute"/> type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractDeleteTestCourseAttribute : BeforeAfterTestAttribute
{
    public static Course Course { get; private set; } = null!;

    /// <summary>
    /// Performs the <see cref="Before"/> operation.
    /// </summary>
    /// <param name="methodUnderTest">The <paramref name="methodUnderTest"/> argument.</param>
    public override void Before(MethodInfo methodUnderTest)
    {
        const string courseTitle = "Course for Delete Test";

        var courseId = Guid.NewGuid();
        var appUserId = Guid.NewGuid();

        Course = CourseExtractor.Extract(courseId, appUserId, courseTitle);
    }

    /// <summary>
    /// Performs the <see cref="After"/> operation.
    /// </summary>
    /// <param name="methodUnderTest">The <paramref name="methodUnderTest"/> argument.</param>
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
