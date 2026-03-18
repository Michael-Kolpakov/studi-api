using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Persistence;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Database;
using Teachio.WebApi;
using Teachio.XIntegrationTests.Base;
using Teachio.XIntegrationTests.Utils;
using Teachio.XIntegrationTests.Utils.Clients;
using Teachio.XIntegrationTests.Utils.Helpers;

namespace Teachio.XIntegrationTests.ControllerTests;

/// <summary>
/// Represents the <see cref="BaseControllerTests{TClient}"/> type.
/// </summary>
/// <typeparam name="TClient">The type of client.</typeparam>
public abstract class BaseControllerTests<TClient> : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private bool _disposed;

    protected TClient Client { get; set; }

    protected BaseControllerTests(CustomWebApplicationFactory<Program> factory, string secondPartUrl)
    {
        Client = ClientInitializer<TClient>.Initialize(factory.CreateClient(), secondPartUrl);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    public static SqlDbHelper GetSqlDbHelper()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TeachioDbContext>()
            .UseInMemoryDatabase(DatabaseConstants.IntegrationTestsInMemoryDatabase, IntegrationTestDatabaseRoot.Root);

        return new SqlDbHelper(optionsBuilder.Options);
    }

    /// <summary>
    /// Releases resources used by this test instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing && Client is IDisposable disposableClient)
        {
            disposableClient.Dispose();
        }

        _disposed = true;
    }
}

/// <summary>
/// Represents the <see cref="BaseControllerTests"/> type.
/// </summary>
public class BaseControllerTests(CustomWebApplicationFactory<Program> factory, string secondPartUrl = "")
    : BaseControllerTests<BaseClient>(factory, secondPartUrl);
