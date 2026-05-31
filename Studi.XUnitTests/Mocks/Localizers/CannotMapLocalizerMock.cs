using Studi.BLL.SharedResource;

namespace Studi.XUnitTests.Mocks.Localizers;

public class CannotMapLocalizerMock : BaseLocalizerMock<CannotMapSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                0, [
                    "CannotMapNullToCourse",
                    "CannotMapNullToSection",
                    "CannotMapNullToVideo"
                ]
            }
        };

        return groupedErrors;
    }
}
