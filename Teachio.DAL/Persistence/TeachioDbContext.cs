using Microsoft.EntityFrameworkCore;

namespace Teachio.DAL.Persistence;

public class TeachioDbContext : DbContext
{
    public TeachioDbContext()
    {
    }

    public TeachioDbContext(DbContextOptions<TeachioDbContext> options)
        : base(options)
    {
    }
}
