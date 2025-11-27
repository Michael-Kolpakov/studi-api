using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Persistence;
using Teachio.DAL.Persistence.Seed;
using ILogger = Serilog.ILogger;

namespace Teachio.WebApi.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = typeof(TeachioDbContext).Assembly.GetName().Name;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TeachioDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                opt => opt.MigrationsAssembly(migrationsAssembly)));

        return services;
    }

    public static async Task InitializeDatabase(IApplicationBuilder app)
    {
        var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var teachioDbContext = serviceScope.ServiceProvider.GetRequiredService<TeachioDbContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>();

        await teachioDbContext.Database.MigrateAsync();
        await TeachioDbSeed.SeedAsync(teachioDbContext, logger);
    }
}
