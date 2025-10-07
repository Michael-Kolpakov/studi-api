namespace Teachio.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public int SaveChanges();

    public Task<int> SaveChangesAsync();
}
