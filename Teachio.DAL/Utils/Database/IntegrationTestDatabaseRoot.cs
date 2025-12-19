using Microsoft.EntityFrameworkCore.Storage;

namespace Teachio.DAL.Utils.Database;

public static class IntegrationTestDatabaseRoot
{
    public static InMemoryDatabaseRoot Root { get; } = new();
}
