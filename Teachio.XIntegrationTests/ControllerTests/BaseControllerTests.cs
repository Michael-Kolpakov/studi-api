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

public abstract class BaseControllerTests<TClient> : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private bool _disposed;

    protected TClient Client { get; set; }

    protected BaseControllerTests(CustomWebApplicationFactory<Program> factory, string secondPartUrl)
    {
        Client = ClientInitializer<TClient>.Initialize(factory.CreateClient(), secondPartUrl);
    }

    public static SqlDbHelper GetSqlDbHelper()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TeachioDbContext>()
            .UseInMemoryDatabase(DatabaseConstants.IntegrationTestsInMemoryDatabase, IntegrationTestDatabaseRoot.Root);

        return new SqlDbHelper(optionsBuilder.Options);
    }

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

public class BaseControllerTests(CustomWebApplicationFactory<Program> factory, string secondPartUrl = "")
    : BaseControllerTests<BaseClient>(factory, secondPartUrl);
