using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
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
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
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
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        return await GetQueryable(predicate, include).ToListAsync();
    }

    public async Task<PaginationResponse<T>> GetAllPaginatedAsync(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null)
    {
        var query = GetQueryable(
            predicate,
            include,
            selector,
            ascendingSortKeySelector,
            descendingSortKeySelector);

        var totalItems = await query.CountAsync();

        IEnumerable<T> items;

        if (pageNumber is null && pageSize is null)
        {
            items = await query.ToListAsync();
        }
        else if (pageNumber == 0 || pageSize == 0)
        {
            items = [];
        }
        else
        {
            var resolvedPageNumber = pageNumber ?? 1;
            var resolvedPageSize = pageSize ?? totalItems;
            var offset = (resolvedPageNumber - 1) * resolvedPageSize;

            items = await query
                .Skip(offset)
                .Take(resolvedPageSize)
                .ToListAsync();
        }

        return PaginationResponse<T>.Create(items, totalItems, pageNumber, pageSize);
    }

    public async Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        return await GetQueryable(predicate, include).SingleOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        return await GetQueryable(predicate, include).FirstOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        return await GetQueryable(predicate, include, selector).FirstOrDefaultAsync();
    }

    public async Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).SingleOrDefaultAsync();
    }

    public async Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).FirstOrDefaultAsync();
    }

    public async Task<List<TResult>> GetProjectedListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
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

    public async Task<int> GetSelfCountAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var query = GetQueryable(predicate, include);
        var count = await query.CountAsync();

        return count;
    }

    public async Task<int> GetNavigationCollectionsCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var query = GetQueryable(predicate, include);
        var elementsCount = await query.SelectMany(collectionSelector).CountAsync();

        return elementsCount;
    }

    public async Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        where TKey : notnull
    {
        var query = GetQueryable(predicate, include);
        var projection = BuildNavigationProjection(query, keySelector, collectionSelector);

        return await projection
            .SelectMany(
                x => x.Collection.DefaultIfEmpty(),
                (projectionItem, collectionItem) => new { projectionItem.Key, CollectionItem = collectionItem })
            .GroupBy(x => x.Key)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Count(item => item.CollectionItem != null));
    }

    private static IQueryable<NavigationCollectionProjection<TKey, TProperty>> BuildNavigationProjection<TKey, TProperty>(
        IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector)
    {
        var entityParameter = Expression.Parameter(typeof(T), "entity");
        var rewrittenKeySelector = ReplaceParameter(keySelector, entityParameter);
        var rewrittenCollectionSelector = ReplaceParameter(collectionSelector, entityParameter);

        var projectionConstructor = typeof(NavigationCollectionProjection<TKey, TProperty>)
            .GetConstructor([typeof(TKey), typeof(IEnumerable<TProperty>)])!;

        var projectionBody = Expression.New(
            projectionConstructor,
            rewrittenKeySelector.Body,
            rewrittenCollectionSelector.Body);

        var projectionLambda = Expression.Lambda<Func<T, NavigationCollectionProjection<TKey, TProperty>>>(
            projectionBody,
            entityParameter);

        return query.Select(projectionLambda);
    }

    private static Expression<Func<T, TResult>> ReplaceParameter<TResult>(
        Expression<Func<T, TResult>> expression,
        ParameterExpression newParameter)
    {
        var visitor = new ReplaceParameterVisitor(expression.Parameters[0], newParameter);
        var updatedBody = visitor.Visit(expression.Body);

        return Expression.Lambda<Func<T, TResult>>(updatedBody, newParameter);
    }

    private sealed record NavigationCollectionProjection<TKey, TProperty>(
        TKey Key,
        IEnumerable<TProperty> Collection);

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }

    private IQueryable<T> GetQueryable(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
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
