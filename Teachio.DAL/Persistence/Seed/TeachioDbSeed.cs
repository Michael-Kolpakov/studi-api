using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.VideoProgress;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Persistence.Seed;

public static class TeachioDbSeed
{
    public static async Task SeedAsync(TeachioDbContext dbContext, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);

        logger.Debug("Staring database seeding...");

        await SeedEntityAsync(dbContext, dbContext.VideoProgress, nameof(VideoProgress), logger);
        await SeedEntityAsync(dbContext, dbContext.Videos, nameof(Video), logger);
        await SeedEntityAsync(dbContext, dbContext.Sections, nameof(Section), logger);
        await SeedEntityAsync(dbContext, dbContext.Courses, nameof(Course), logger);
        await SeedEntityAsync(dbContext, dbContext.AppUsers, nameof(AppUser), logger);

        logger.Debug("Database seeding completed.");
    }

    private static async Task SeedEntityAsync<TEntity>(
        TeachioDbContext dbContext,
        DbSet<TEntity> dbSet,
        string jsonFileName,
        ILogger logger)
        where TEntity : class
    {
        if (await dbSet.AnyAsync())
        {
            logger.Debug("Skipping seeding for {EntityName} table as it already contains data.", jsonFileName);

            return;
        }

        var assemblyPath = Path.GetDirectoryName(typeof(TeachioDbSeed).Assembly.Location);
        var fullPath = Path.Combine(assemblyPath!, $@"Persistence\Seed\Content\{jsonFileName}.json");
        var jsonData = await File.ReadAllTextAsync(fullPath);

        var entities = JsonSerializer.Deserialize<List<TEntity>>(jsonData);

        if (entities is null || entities.Count == 0)
        {
            logger.Debug("No data found in {EntityName}.json to seed.", jsonFileName);

            return;
        }

        await dbSet.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();

        logger.Debug("Seeded {Count} records into {EntityName} table.", entities.Count, jsonFileName);
    }
}
