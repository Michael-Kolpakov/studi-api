using Newtonsoft.Json;

namespace Teachio.XIntegrationTests.Utils;

/// <summary>
/// Represents the <see cref="TestDataProvider"/> type.
/// </summary>
public static class TestDataProvider
{
    /// <summary>
    /// Performs this operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <returns>The result produced by this operation.</returns>
    public static TEntity GetTestData<TEntity>()
    {
        var typeName = typeof(TEntity).Name;
        var jsonFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", $"{typeName}.json");

        using var reader = new StreamReader(jsonFilePath);

        var fileJson = reader.ReadToEnd();
        var entity = JsonConvert.DeserializeObject<TEntity>(fileJson)!;

        return entity;
    }
}
