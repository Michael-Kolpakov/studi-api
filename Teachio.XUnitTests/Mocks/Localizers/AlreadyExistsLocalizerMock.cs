using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

public class AlreadyExistsLocalizerMock : BaseLocalizerMock<AlreadyExistsSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                1, [
                    "CourseAlreadyExistsForUser"
                ]
            },
            {
                2, [
                    "CourseAlreadyExistsForUserWithId",
                    "SectionAlreadyExistsForCourse"
                ]
            }
        };

        return groupedErrors;
    }
}
