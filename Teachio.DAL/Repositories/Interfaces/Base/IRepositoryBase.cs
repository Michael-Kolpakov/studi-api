using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Repositories.Interfaces.Base;

public interface IRepositoryBase<T>
    where T : class
{
    T Create(T entity);

    Task<T> CreateAsync(T entity);

    Task CreateRangeAsync(IEnumerable<T> items);

    T Update(T entity);

    void UpdateRange(IEnumerable<T> items);

    T Delete(T entity);

    void DeleteRange(IEnumerable<T> items);

    void Attach(T entity);

    EntityEntry<T> Entry(T entity);

    Task ExecuteSqlRaw(string query);

    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<PaginationResponse<T>> GetAllPaginatedAsync(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null);

    Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        int? offset = null);

    Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null);

    Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null);

    Task<List<TResult>> GetProjectedListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null);

    Task<int> GetSelfCountAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<int> GetNavigationCollectionsCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        where TKey : notnull;

    IQueryable<T> FindAll(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
}
