using Microsoft.EntityFrameworkCore;
using Studi.DAL.Persistence;
using Studi.DAL.Persistence.Seed;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Database;
using ILogger = Serilog.ILogger;

namespace Studi.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StudiDbContext>((serviceProvider, options) =>
        {
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            var migrationsAssembly = typeof(StudiDbContext).Assembly.GetName().Name;

            if (string.Equals(environment.EnvironmentName, "IntegrationTests", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase(DatabaseConstants.IntegrationTestsInMemoryDatabase, IntegrationTestDatabaseRoot.Root);

                return;
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly));
        });

        return services;
    }

    public static async Task InitializeDatabase(IApplicationBuilder app, CancellationToken cancellationToken)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();

        if (string.Equals(environment.EnvironmentName, "IntegrationTests", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var dbContext = services.GetRequiredService<StudiDbContext>();
        var logger = services.GetRequiredService<ILogger>();

        try
        {
            if (!await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                logger.Fatal("Database is not reachable. Aborting startup.");
                throw new InvalidOperationException("Cannot connect to database.");
            }

            var pending = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pending.Count != 0)
            {
                logger.Information("Pending migrations ({Count}): {PendingMigrations}", pending.Count, string.Join(", ", pending));

                var appliedBefore = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToHashSet();

                await dbContext.Database.MigrateAsync(cancellationToken);

                logger.Information("Migrations applied successfully.");

                var appliedAfter = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();

                var newlyApplied = appliedAfter.Except(appliedBefore).ToList();

                if (newlyApplied.Count != 0)
                {
                    logger.Information("Newly applied migrations: {NewlyAppliedMigrations}", string.Join(", ", newlyApplied));
                }
                else
                {
                    logger.Information("No new migrations were applied.");
                }
            }
            else
            {
                logger.Information("No pending migrations.");
            }

            await StudiDbSeed.SeedAsync(dbContext, logger, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.Warning("Database initialization was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}
