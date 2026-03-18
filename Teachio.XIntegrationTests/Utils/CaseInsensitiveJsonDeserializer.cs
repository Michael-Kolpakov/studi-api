using System.Text.Json;

namespace Teachio.XIntegrationTests.Utils;

/// <summary>
/// Represents the <see cref="CaseInsensitiveJsonDeserializer"/> type.
/// </summary>
public static class CaseInsensitiveJsonDeserializer
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static TValue? Deserialize<TValue>(string? text)
        where TValue : class
    {
        if (text is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<TValue>(text, _options);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
