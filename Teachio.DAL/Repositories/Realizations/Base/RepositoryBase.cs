using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
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

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entityEntry = await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

        return entityEntry.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<T>().AddRangeAsync(items, cancellationToken);
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

    public Task ExecuteSqlRaw(string query, CancellationToken cancellationToken = default)
    {
        return _dbContext.Database.ExecuteSqlRawAsync(query, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).ToListAsync(cancellationToken);
    }

    public async Task<PaginationResponse<T>> GetAllPaginatedAsync(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        Expression<Func<T, object>>? secondaryAscendingSortKeySelector = null,
        Expression<Func<T, object>>? secondaryDescendingSortKeySelector = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(
            predicate,
            include,
            selector,
            ascendingSortKeySelector,
            descendingSortKeySelector,
            secondaryAscendingSortKeySelector,
            secondaryDescendingSortKeySelector);

        var totalItems = await query.CountAsync(cancellationToken);

        IEnumerable<T> items;

        if (pageNumber is null && pageSize is null)
        {
            items = await query.ToListAsync(cancellationToken);
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
                .ToListAsync(cancellationToken);
        }

        return PaginationResponse<T>.Create(items, totalItems, pageNumber, pageSize);
    }

    public async Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include, selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetProjectedListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(
                predicate,
                include,
                selector,
                ascendingSortKeySelector,
                descendingSortKeySelector,
                offset: offset)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetSelfCountAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(predicate, include);
        var count = await query.CountAsync(cancellationToken);

        return count;
    }

    public async Task<int> GetNavigationCollectionsCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(predicate, include);
        var elementsCount = await query.SelectMany(collectionSelector).CountAsync(cancellationToken);

        return elementsCount;
    }

    public async Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
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
                g => g.Count(item => item.CollectionItem != null),
                cancellationToken);
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
        Expression<Func<T, object>>? secondaryAscendingSortKeySelector = null,
        Expression<Func<T, object>>? secondaryDescendingSortKeySelector = null,
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
            var ordered = query.OrderBy(ascendingSortKeySelector);

            if (secondaryAscendingSortKeySelector is not null)
            {
                ordered = ordered.ThenBy(secondaryAscendingSortKeySelector);
            }
            else if (secondaryDescendingSortKeySelector is not null)
            {
                ordered = ordered.ThenByDescending(secondaryDescendingSortKeySelector);
            }

            query = ordered;
        }
        else if (descendingSortKeySelector is not null)
        {
            var ordered = query.OrderByDescending(descendingSortKeySelector);

            if (secondaryDescendingSortKeySelector is not null)
            {
                ordered = ordered.ThenByDescending(secondaryDescendingSortKeySelector);
            }
            else if (secondaryAscendingSortKeySelector is not null)
            {
                ordered = ordered.ThenBy(secondaryAscendingSortKeySelector);
            }

            query = ordered;
        }
        else
        {
            if (secondaryAscendingSortKeySelector is not null)
            {
                query = query.OrderBy(secondaryAscendingSortKeySelector);
            }
            else if (secondaryDescendingSortKeySelector is not null)
            {
                query = query.OrderByDescending(secondaryDescendingSortKeySelector);
            }
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
