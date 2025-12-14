using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

public class AlreadyExistsLocalizerMock : BaseLocalizerMock<AlreadyExistsSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                2, [
                    "CourseAlreadyExists"
                ]
            }
        };

        return groupedErrors;
    }
}
