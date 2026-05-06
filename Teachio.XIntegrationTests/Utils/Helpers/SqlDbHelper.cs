using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Teachio.DAL.Persistence;

namespace Teachio.XIntegrationTests.Utils.Helpers;

public class SqlDbHelper : IDisposable
{
    private const string IdPropertyName = "Id";

    private static readonly ConcurrentDictionary<Type, object> _entityLocks = new ConcurrentDictionary<Type, object>();
    private readonly TeachioDbContext _dbContext;
    private readonly Lock _lock = new Lock();
    private bool _disposed;

    public SqlDbHelper(DbContextOptions<TeachioDbContext> options)
    {
        _dbContext = new TeachioDbContext(options);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public string GetIdentityInsertString<TEntity>(bool isEnabled)
    {
        var value = isEnabled ? "ON" : "OFF";
        var identityInsertString = $"SET IDENTITY_INSERT {GetItemTableName<TEntity>()} {value};";

        return identityInsertString;
    }

    public bool CheckIfExistsById<TEntity>(int id)
        where TEntity : class, new()
    {
        var idProperty = typeof(TEntity).GetProperty(IdPropertyName);
        var entity = _dbContext
            .Set<TEntity>()
            .AsEnumerable()
            .FirstOrDefault(s => (int)idProperty?.GetValue(s)! == id);

        var entityExists = entity != null;

        return entityExists;
    }

    public TEntity? GetItemWithId<TEntity>(Guid id)
        where TEntity : class, new()
    {
        var idProperty = typeof(TEntity).GetProperty(IdPropertyName);
        var entity = _dbContext
            .Set<TEntity>()
            .AsEnumerable()
            .FirstOrDefault(s => (Guid)idProperty?.GetValue(s)! == id);

        return entity;
    }

    public TEntity? GetItemWithPredicate<TEntity>(Func<TEntity, bool>? predicate = null)
        where TEntity : class, new()
    {
        return predicate != null
            ? _dbContext.Set<TEntity>().AsEnumerable().FirstOrDefault(predicate)
            : _dbContext.Set<TEntity>().FirstOrDefault();
    }

    public IEnumerable<TEntity> GetAllItems<TEntity>(Func<TEntity, bool>? predicate = null)
        where TEntity : class
    {
        return predicate != null
            ? _dbContext.Set<TEntity>().AsNoTracking().AsEnumerable().Where(predicate)
            : _dbContext.Set<TEntity>().AsNoTracking();
    }

    public bool Any<TEntity>(Func<TEntity, bool>? predicate = null)
        where TEntity : class, new()
    {
        return predicate != null
            ? _dbContext.Set<TEntity>().AsNoTracking().AsEnumerable().Any(predicate)
            : _dbContext.Set<TEntity>().AsNoTracking().Any();
    }

    public TEntity AddItem<TEntity>(TEntity newItem)
        where TEntity : class, new()
    {
        var idProperty = typeof(TEntity).GetProperty(IdPropertyName);

        if (idProperty != null)
        {
            var value = idProperty.GetValue(newItem) as string;

            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out _))
            {
                idProperty.SetValue(newItem, 0);
            }
        }

        lock (_entityLocks.GetOrAdd(typeof(TEntity), new object()))
        {
            return _dbContext.Set<TEntity>().Add(newItem).Entity;
        }
    }

    public void AddItemWithCustomId<TEntity>(TEntity newItem)
        where TEntity : class, new()
    {
        object entityLock;

        lock (_lock)
        {
            entityLock = _entityLocks.GetOrAdd(typeof(TEntity), new object());
        }

        lock (entityLock)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity == newItem);

            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }

            var isRelational = _dbContext.Database.IsRelational();

            if (!isRelational)
            {
                _dbContext.Add(newItem);
                _dbContext.SaveChanges();

                return;
            }

            try
            {
                _dbContext.Database.OpenConnection();

                var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
                var tableSchema = entityType?.GetSchema();
                var tableName = entityType?.GetTableName();
                var qualifiedTableName = string.IsNullOrEmpty(tableSchema)
                    ? tableName
                    : $"{tableSchema}.{tableName}";

                var identityCommand = $"SET IDENTITY_INSERT {qualifiedTableName} {{0}}";

                if (TableHasIdentityColumn(typeof(TEntity)))
                {
                    _dbContext.Database.ExecuteSqlRaw(identityCommand, "ON");
                }

                _dbContext.Add(newItem);
                _dbContext.SaveChanges();

                if (TableHasIdentityColumn(typeof(TEntity)))
                {
                    _dbContext.Database.ExecuteSqlRaw(identityCommand, "OFF");
                }
            }
            finally
            {
                _dbContext.Database.CloseConnection();
            }
        }
    }

    public TEntity DeleteItem<TEntity>(TEntity item)
        where TEntity : class, new()
    {
        lock (_entityLocks.GetOrAdd(typeof(TEntity), new object()))
        {
            return _dbContext.Set<TEntity>().Remove(item).Entity;
        }
    }

    public void SaveChanges()
    {
        lock (_lock)
        {
            _dbContext.SaveChanges();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }

    private string GetItemTableName<TEntity>()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
        var tableName = string.Join(".", entityType?.GetSchema(), entityType?.GetTableName());

        return tableName;
    }

    private bool TableHasIdentityColumn(Type entityType)
    {
        var entityTypeMeta = _dbContext.Model.FindEntityType(entityType);

        return entityTypeMeta != null && entityTypeMeta.GetProperties().Any(p => p.ValueGenerated == ValueGenerated.OnAdd);
    }
}
