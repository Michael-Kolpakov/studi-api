using Studi.BLL.SharedResource;

namespace Studi.XUnitTests.Mocks.Localizers;

public class BllLocalizerMock : BaseLocalizerMock<BllSharedResource>
{
    protected override Dictionary<int, List<string>> DefineGroupedErrors()
    {
        var groupedErrors = new Dictionary<int, List<string>>()
        {
            {
                1, [
                    "TitleFilterLengthTooLong"
                ]
            }
        };

        return groupedErrors;
    }
}
