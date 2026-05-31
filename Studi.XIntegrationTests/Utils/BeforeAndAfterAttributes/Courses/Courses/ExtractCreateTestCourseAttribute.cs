using System.Reflection;
using Studi.BLL.DTOs.Courses.Courses.Request.Create;
using Studi.DAL.Entities.Courses.Courses;
using Studi.XIntegrationTests.ControllerTests;
using Xunit.Sdk;

namespace Studi.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractCreateTestCourseAttribute : BeforeAfterTestAttribute
{
    public static CourseCreateRequestDto CourseCreateRequestDto { get; private set; } = null!;

    public override void Before(MethodInfo methodUnderTest)
    {
        CourseCreateRequestDto = new CourseCreateRequestDto()
        {
            Title = "CourseCreateRequestDto for Create Test",
            Description = "Description of CourseCreateRequestDto for Create Test"
        };
    }

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
