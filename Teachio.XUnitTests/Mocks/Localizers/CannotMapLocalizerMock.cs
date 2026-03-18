using Teachio.BLL.SharedResource;

namespace Teachio.XUnitTests.Mocks.Localizers;

/// <summary>
/// Represents the <see cref="CannotMapLocalizerMock"/> type.
/// </summary>
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
