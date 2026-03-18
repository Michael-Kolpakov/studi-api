using System.Reflection;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.XIntegrationTests.ControllerTests;
using Xunit.Sdk;

namespace Teachio.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

/// <summary>
/// Represents the <see cref="ExtractCreateTestCourseAttribute"/> type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractCreateTestCourseAttribute : BeforeAfterTestAttribute
{
    public static CourseCreateRequestDto CourseCreateRequestDto { get; private set; } = null!;

    /// <summary>
    /// Performs the <see cref="Before"/> operation.
    /// </summary>
    /// <param name="methodUnderTest">The <paramref name="methodUnderTest"/> argument.</param>
    public override void Before(MethodInfo methodUnderTest)
    {
        CourseCreateRequestDto = new CourseCreateRequestDto()
        {
            Title = "CourseCreateRequestDto for Create Test",
            Description = "Description of CourseCreateRequestDto for Create Test",
            ThumbnailName = "coursecreaterequestdto-for-create-test.png"
        };
    }

    /// <summary>
    /// Performs the <see cref="After"/> operation.
    /// </summary>
    /// <param name="methodUnderTest">The <paramref name="methodUnderTest"/> argument.</param>
    public override void After(MethodInfo methodUnderTest)
    {
        var sqlDbHelper = BaseControllerTests.GetSqlDbHelper();
        var course = sqlDbHelper.GetItemWithPredicate<Course>(course => course.Title == CourseCreateRequestDto.Title);

        if (course is not null)
        {
            sqlDbHelper.DeleteItem(course);
        }
    }
}
