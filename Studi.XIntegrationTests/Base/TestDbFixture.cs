using Microsoft.EntityFrameworkCore;
using Studi.DAL.Persistence;

namespace Studi.XIntegrationTests.Base;

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

    private static StudiDbContext CreateContext(string connectionString)
    {
        return new StudiDbContext(
            new DbContextOptionsBuilder<StudiDbContext>()
                .UseSqlServer(connectionString)
                .Options);
    }
}
