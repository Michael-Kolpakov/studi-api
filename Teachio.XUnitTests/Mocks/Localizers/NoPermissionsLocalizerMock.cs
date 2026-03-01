using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

public class NoPermissionsLocalizerMock : BaseLocalizerMock<NoPermissionsSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                1, [
                    "NoPermissionsToDeleteCourseForUser",
                    "NoPermissionsToUpdateCourseForUser"
                ]
            },
            {
                2, [
                    "NoPermissionsToDeleteCourseForUserWithId",
                    "NoPermissionsToUpdateCourseForUserWithId"
                ]
            }
        };

        return groupedErrors;
    }
}
