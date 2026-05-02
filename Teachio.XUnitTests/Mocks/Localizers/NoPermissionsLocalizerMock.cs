using Teachio.BLL.SharedResources;

namespace Teachio.XUnitTests.Mocks.Localizers;

public class NoPermissionsLocalizerMock : BaseLocalizerMock<NoPermissionsSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                0, [
                    "NoPermissionsToCreateSectionForCourseOfAnotherUser",
                    "NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUser"
                ]
            },
            {
                1, [
                    "NoPermissionsToDeleteCourseForUser",
                    "NoPermissionsToUpdateCourseForUser",
                    "NoPermissionsToDeleteSectionForUser",
                    "NoPermissionsToUpdateSectionForUser",
                    "NoPermissionsToDeleteVideoForUser",
                    "NoPermissionsToUpdateVideoForUser",
                    "NoPermissionsToUpdateVideoProgressForUser"
                ]
            },
            {
                2, [
                    "NoPermissionsToDeleteCourseForUserWithId",
                    "NoPermissionsToUpdateCourseForUserWithId",
                    "NoPermissionsToDeleteSectionForUserWithId",
                    "NoPermissionsToUpdateSectionForUserWithId",
                    "NoPermissionsToDeleteVideoForUserWithId",
                    "NoPermissionsToUpdateVideoForUserWithId",
                    "NoPermissionsToUpdateVideoProgressForUserWithId"
                ]
            },
            {
                3, [
                    "NoPermissionsToCreateSectionForCourseOfAnotherUserWithId"
                ]
            },
            {
                4, [
                    "NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUserWithId"
                ]
            }
        };

        return groupedErrors;
    }
}
