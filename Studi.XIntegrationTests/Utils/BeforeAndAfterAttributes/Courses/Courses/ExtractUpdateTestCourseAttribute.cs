using System.Reflection;
using Studi.BLL.DTOs.Courses.Courses.Request.Update;
using Studi.DAL.Entities.Courses.Courses;
using Studi.XIntegrationTests.Utils.Extractors;
using Xunit.Sdk;

namespace Studi.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ExtractUpdateTestCourseAttribute : BeforeAfterTestAttribute
{
    public static CourseUpdateRequestDto CourseUpdateRequestDto { get; private set; } = null!;

    private static Course PersistedCourse { get; set; } = null!;

    public override void Before(MethodInfo methodUnderTest)
    {
        var courseId = Guid.Parse("9d5400a6-9fba-48a1-b18c-517f2bd52db9");
        var appUserId = Guid.Parse("34cff13a-867f-4424-bcaf-fd8ec958359f");

        PersistedCourse = CourseExtractor.Extract(courseId, appUserId);

        CourseUpdateRequestDto = new CourseUpdateRequestDto()
        {
            Id = PersistedCourse.Id,
            Title = "CourseUpdateRequestDto for Update Test",
            Description = "Description of CourseUpdateRequestDto for Update Test"
        };
    }

    public override void After(MethodInfo methodUnderTest)
    {
        CourseExtractor.Remove(PersistedCourse);
    }
}
