using Microsoft.EntityFrameworkCore;
using Teachio.DAL.Persistence;

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

    public static void InitializeDatabase(IApplicationBuilder app)
    {
        var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var teachioDbContext = serviceScope.ServiceProvider.GetRequiredService<TeachioDbContext>();

        teachioDbContext.Database.Migrate();
        // TeachioDbContext.SeedAsync(teachioDbContext).Wait();
    }
}
