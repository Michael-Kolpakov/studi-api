using Microsoft.EntityFrameworkCore.Storage;

namespace Teachio.DAL.Utils.Database;

/// <summary>
/// Represents the <see cref="IntegrationTestDatabaseRoot"/> type.
/// </summary>
public static class IntegrationTestDatabaseRoot
{
    public static InMemoryDatabaseRoot Root { get; } = new();
}
