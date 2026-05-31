using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.ThumbnailFiles;
using Studi.DAL.Entities.Courses.Videos.VideoFiles;
using Studi.DAL.Entities.Courses.Videos.VideoProgress;
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Entities.Shared;
using Studi.DAL.Entities.Users.AvatarFiles;
using Studi.DAL.Entities.Users.Users;

namespace Studi.DAL.Persistence.Seed;

public static class StudiDbSeed
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static async Task SeedAsync(
        StudiDbContext dbContext,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information("Starting database seeding...");

        await SeedEntityAsync(dbContext, dbContext.AppUsers, nameof(AppUser), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.AvatarFiles, nameof(AvatarFile), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.Courses, nameof(Course), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.UserCourses, nameof(UserCourse), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.ThumbnailFiles, nameof(ThumbnailFile), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.Sections, nameof(Section), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.Videos, nameof(Video), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.VideoFiles, nameof(VideoFile), logger, cancellationToken);
        await SeedEntityAsync(dbContext, dbContext.VideoProgress, nameof(VideoProgress), logger, cancellationToken);

        logger.Information("Database seeding completed.");
    }

    private static async Task SeedEntityAsync<TEntity>(
        StudiDbContext dbContext,
        DbSet<TEntity> dbSet,
        string jsonFileName,
        ILogger logger,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (await dbSet.AnyAsync(cancellationToken))
        {
            logger.Debug("Skipping seeding for {EntityName} table as it already contains data.", jsonFileName);

            return;
        }

        var assemblyPath = Path.GetDirectoryName(typeof(StudiDbSeed).Assembly.Location);
        var fullPath = Path.Combine(assemblyPath!, "Persistence", "Seed", "Content", $"{jsonFileName}.json");

        if (!File.Exists(fullPath))
        {
            logger.Warning("Seed file for {EntityName} not found at path: {Path}", jsonFileName, fullPath);

            return;
        }

        var jsonData = await File.ReadAllTextAsync(fullPath, cancellationToken);

        var entities = JsonSerializer.Deserialize<List<TEntity>>(jsonData, _jsonOptions);

        if (entities is null || entities.Count == 0)
        {
            logger.Warning("No data found in {EntityName}.json to seed.", jsonFileName);

            return;
        }

        await dbSet.AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.Information("Seeded {Count} records into {EntityName} table.", entities.Count, jsonFileName);
    }
}
