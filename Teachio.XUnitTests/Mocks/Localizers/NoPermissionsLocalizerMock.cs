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
                    "NoPermissionsToCreateSectionForCourseOfAnotherUser",
                    "NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUser"
                ]
            },
            {
                1, [
                    "NoPermissionsToDeleteCourseForUser",
                    "NoPermissionsToGetCourseForUser",
                    "NoPermissionsToUpdateCourseForUser",
                    "NoPermissionsToDeleteSectionForUser",
                    "NoPermissionsToGetSectionForUser",
                    "NoPermissionsToUpdateSectionForUser",
                    "NoPermissionsToDeleteVideoForUser",
                    "NoPermissionsToGetVideoForUser",
                    "NoPermissionsToUpdateVideoForUser",
                    "NoPermissionsToUpdateVideoProgressForUser"
                ]
            },
            {
                2, [
                    "NoPermissionsToDeleteCourseForUserWithId",
                    "NoPermissionsToGetCourseForUserWithId",
                    "NoPermissionsToUpdateCourseForUserWithId",
                    "NoPermissionsToDeleteSectionForUserWithId",
                    "NoPermissionsToGetSectionForUserWithId",
                    "NoPermissionsToUpdateSectionForUserWithId",
                    "NoPermissionsToDeleteVideoForUserWithId",
                    "NoPermissionsToGetVideoForUserWithId",
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
