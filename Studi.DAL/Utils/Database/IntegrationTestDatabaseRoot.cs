using Microsoft.EntityFrameworkCore.Storage;

namespace Studi.DAL.Utils.Database;

public static class IntegrationTestDatabaseRoot
{
    public static InMemoryDatabaseRoot Root { get; } = new();
}
