using Microsoft.Extensions.Localization;

namespace Studi.XUnitTests.Mocks.Localizers;

public abstract class BaseLocalizerMock<TResource> : IStringLocalizer<TResource>
{
    private readonly List<LocalizedString> _localizedStrings;

    private Dictionary<int, List<string>> GroupedErrors => _lazyGroupedErrors.Value;

    private readonly Lazy<Dictionary<int, List<string>>> _lazyGroupedErrors;

    protected BaseLocalizerMock()
    {
        _localizedStrings = new List<LocalizedString>();
        _lazyGroupedErrors = new Lazy<Dictionary<int, List<string>>>(DefineGroupedErrors);
    }

    public LocalizedString this[string name]
    {
        get
        {
            if (GroupedErrors.TryGetValue(0, out var noArgumentErrors) && noArgumentErrors.Contains(name))
            {
                return new LocalizedString(name, $"Error '{name}'");
            }

            throw new ArgumentException($"Cannot find error message '{name}' that accepts no arguments");
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var argumentsCount = arguments.Length;
            if (GroupedErrors.TryGetValue(argumentsCount, out var argumentErrors) && argumentErrors.Contains(name))
            {
                return GetErrorMessage(name, arguments);
            }

            throw new ArgumentException($"Cannot find error message '{name}' that accepts {argumentsCount} arguments");
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _localizedStrings;
    }

    protected abstract Dictionary<int, List<string>> DefineGroupedErrors();

    private static LocalizedString GetErrorMessage(string error, params object[] arguments)
    {
        if (arguments.Length == 0)
        {
            return new LocalizedString(error, $"Error '{error}'");
        }

        var formattedArguments = string.Join(", ", arguments);
        var errorMessage = $"Error '{error}'. Arguments: {formattedArguments}";

        return new LocalizedString(error, errorMessage);
    }
}
