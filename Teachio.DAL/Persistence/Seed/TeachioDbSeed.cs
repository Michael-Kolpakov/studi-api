using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.VideoProgress;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Entities.Users;

namespace Teachio.DAL.Persistence.Seed;

public class TeachioDbSeed
{
    public static async Task SeedAsync(TeachioDbContext dbContext)
    {
        await SeedEntityAsync(dbContext, dbContext.VideoProgress, nameof(VideoProgress));
        await SeedEntityAsync(dbContext, dbContext.Videos, nameof(Video));
        await SeedEntityAsync(dbContext, dbContext.Sections, nameof(Section));
        await SeedEntityAsync(dbContext, dbContext.Courses, nameof(Course));
        await SeedEntityAsync(dbContext, dbContext.AppUsers, nameof(AppUser));
    }

    private static async Task SeedEntityAsync<TEntity>(
        TeachioDbContext dbContext,
        DbSet<TEntity> dbSet,
        string jsonFileName
    ) where TEntity : class
    {
        if (await dbSet.AnyAsync())
        {
            return;
        }

        var assemblyPath = Path.GetDirectoryName(typeof(TeachioDbSeed).Assembly.Location);
        var fullPath = Path.Combine(assemblyPath!, $@"Persistence\Seed\Content\{jsonFileName}.json");
        var jsonData = await File.ReadAllTextAsync(fullPath);

        var entities = JsonSerializer.Deserialize<List<TEntity>>(jsonData);

        if (entities is null || entities.Count == 0)
        {
            return;
        }

        await dbSet.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }
}
