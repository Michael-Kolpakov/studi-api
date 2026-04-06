using Newtonsoft.Json;

namespace Teachio.XIntegrationTests.Utils;

public static class TestDataProvider
{
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
