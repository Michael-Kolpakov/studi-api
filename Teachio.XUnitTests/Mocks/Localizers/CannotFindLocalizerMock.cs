using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

/// <summary>
/// Represents the <see cref="CannotFindLocalizerMock"/> type.
/// </summary>
public class CannotFindLocalizerMock : BaseLocalizerMock<CannotFindSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                1, [
                    "CannotFindCourseById",
                    "CannotFindSectionById",
                    "CannotFindVideoById",
                    "CannotFindVideoProgressById",
                    "CannotFindUserById"
                ]
            },
            {
                2, [
                    "CannotFindCourseByKey",
                    "CannotFindSectionByKey",
                    "CannotFindVideoByKey"
                ]
            }
        };

        return groupedErrors;
    }
}
