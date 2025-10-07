using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly TeachioDbContext _dbContext;

    public RepositoryWrapper(TeachioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
