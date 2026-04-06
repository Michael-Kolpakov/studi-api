using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Persistence;

namespace Teachio.XIntegrationTests.Base;

public class TestDbFixture : IntegrationTestBase
{
    private static readonly Lock _lock = new Lock();
    private static bool _dbIsCreated;
    private readonly string _connectionString;

    public TestDbFixture()
    {
        _connectionString = Environment.GetEnvironmentVariable("DefaultConnection")!;

        lock (_lock)
        {
            if (_dbIsCreated)
            {
                return;
            }

            using (var context = CreateContext(_connectionString))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            _dbIsCreated = true;
        }
    }

    private static TeachioDbContext CreateContext(string connectionString)
    {
        return new TeachioDbContext(
            new DbContextOptionsBuilder<TeachioDbContext>()
                .UseSqlServer(connectionString)
                .Options);
    }
}
