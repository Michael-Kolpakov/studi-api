using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Studi.DAL.Persistence;

namespace Studi.DAL.DesignTime;

public class StudiDesignTimeDbContextFactory : IDesignTimeDbContextFactory<StudiDbContext>
{
    public StudiDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var webApiPath = Path.Combine(basePath, "..", "Studi.WebApi");
        if (Directory.Exists(webApiPath))
        {
            basePath = webApiPath;
        }

        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(basePath, "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(basePath, "appsettings.Local.json"), optional: true);

        var config = configBuilder.Build();

        var conn = config.GetConnectionString("DefaultConnection")
               ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<StudiDbContext>();
        optionsBuilder.UseSqlServer(conn);

        return new StudiDbContext(optionsBuilder.Options);
    }
}
