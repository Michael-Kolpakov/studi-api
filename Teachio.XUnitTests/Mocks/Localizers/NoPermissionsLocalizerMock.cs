using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

public class NoPermissionsLocalizerMock : BaseLocalizerMock<NoPermissionsSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                0, [
                    "NoPermissionsToCreateSectionForCourseOfAnotherUser"
                ]
            },
            {
                1, [
                    "NoPermissionsToDeleteCourseForUser",
                    "NoPermissionsToUpdateCourseForUser",
                    "NoPermissionsToDeleteSectionForUser"
                ]
            },
            {
                2, [
                    "NoPermissionsToDeleteCourseForUserWithId",
                    "NoPermissionsToUpdateCourseForUserWithId",
                    "NoPermissionsToDeleteSectionForUserWithId"
                ]
            },
            {
                3, [
                    "NoPermissionsToCreateSectionForCourseOfAnotherUserWithId"
                ]
            }
        };

        return groupedErrors;
    }
}
