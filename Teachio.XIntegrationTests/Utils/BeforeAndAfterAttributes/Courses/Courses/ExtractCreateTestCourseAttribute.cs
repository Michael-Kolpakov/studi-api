using System.Reflection;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Users;
using Teachio.XIntegrationTests.ControllerTests;
using Teachio.XIntegrationTests.Utils.Extractors;
using Xunit.Sdk;

namespace Teachio.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractCreateTestCourseAttribute : BeforeAfterTestAttribute
{
    public static CourseCreateRequestDto CourseCreateRequestDto { get; private set; } = null!;

    private static AppUser OwnerUser { get; set; } = null!;

    public override void Before(MethodInfo methodUnderTest)
    {
        var ownerUserId = Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        OwnerUser = AppUserExtractor.Extract(ownerUserId);
        CourseCreateRequestDto = new CourseCreateRequestDto()
        {
            Title = "CourseCreateRequestDto for Create Test",
            Description = "Description of CourseCreateRequestDto for Create Test",
            ThumbnailName = "coursecreaterequestdto-for-create-test.png",
            OwnerUserId = OwnerUser.Id
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

        AppUserExtractor.Remove(OwnerUser);
    }
}
