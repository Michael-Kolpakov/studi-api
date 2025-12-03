using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Repositories.Realizations.Base;

public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    private readonly TeachioDbContext _dbContext;

    protected RepositoryBase(TeachioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<T> FindAll(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return GetQueryable(predicate, include).AsNoTracking();
    }

    public T Create(T entity)
   {
        return _dbContext.Set<T>().Add(entity).Entity;
    }

    public async Task<T> CreateAsync(T entity)
    {
        var entityEntry = await _dbContext.Set<T>().AddAsync(entity);

        return entityEntry.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<T> items)
    {
        return _dbContext.Set<T>().AddRangeAsync(items);
    }

    public T Update(T entity)
    {
        return _dbContext.Set<T>().Update(entity).Entity;
    }

    public void UpdateRange(IEnumerable<T> items)
    {
        _dbContext.Set<T>().UpdateRange(items);
    }

    public T Delete(T entity)
    {
        return _dbContext.Set<T>().Remove(entity).Entity;
    }

    public void DeleteRange(IEnumerable<T> items)
    {
        _dbContext.Set<T>().RemoveRange(items);
    }

    public void Attach(T entity)
    {
        _dbContext.Set<T>().Attach(entity);
    }

    public EntityEntry<T> Entry(T entity)
    {
        return _dbContext.Entry(entity);
    }

    public Task ExecuteSqlRaw(string query)
    {
        return _dbContext.Database.ExecuteSqlRawAsync(query);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return await GetQueryable(predicate, include).ToListAsync();
    }

    public PaginationResponse<T> GetAllPaginated(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = default,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default,
        Expression<Func<T, object>>? ascendingSortKeySelector = default,
        Expression<Func<T, object>>? descendingSortKeySelector = default)
    {
        var query = GetQueryable(
            predicate,
            include,
            selector,
            ascendingSortKeySelector,
            descendingSortKeySelector);

        var paginationResponse = PaginationResponse<T>.Create(
            query,
            pageNumber,
            pageSize);

        return paginationResponse;
    }

    public async Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return await GetQueryable(predicate, include).SingleOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return await GetQueryable(predicate, include).FirstOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return await GetQueryable(predicate, include, selector).FirstOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default,
        Expression<Func<T, object>>? ascendingSortKeySelector = default,
        Expression<Func<T, object>>? descendingSortKeySelector = default,
        int? offset = null)
    {
        return await GetQueryable(
                predicate,
                include,
                selector,
                ascendingSortKeySelector,
                descendingSortKeySelector,
                offset: offset)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetCountAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        return await GetQueryable(predicate, include).CountAsync();
    }

    public async Task<int> GetNavigationCollectionCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        var query = GetQueryable(predicate, include);
        var elements = query.SelectMany(collectionSelector);

        return await elements.CountAsync();
    }

    private IQueryable<T> GetQueryable(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default,
        Expression<Func<T, T>>? selector = default,
        Expression<Func<T, object>>? ascendingSortKeySelector = default,
        Expression<Func<T, object>>? descendingSortKeySelector = default,
        int? limit = null,
        int? offset = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (selector is not null)
        {
            query = query.Select(selector);
        }

        if (ascendingSortKeySelector is not null)
        {
            query = query.OrderBy(ascendingSortKeySelector);
        }

        if (descendingSortKeySelector is not null)
        {
            query = query.OrderByDescending(descendingSortKeySelector);
        }

        if (offset is >= 0)
        {
            query = query.Skip(offset.Value);
        }

        if (limit is > 0)
        {
            query = query.Take(limit.Value);
        }

        return query.AsNoTracking();
    }
}
