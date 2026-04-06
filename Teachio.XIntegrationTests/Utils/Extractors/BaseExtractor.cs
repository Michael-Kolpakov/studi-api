using Teachio.XIntegrationTests.ControllerTests;
using Teachio.XIntegrationTests.Utils.Helpers;

namespace Teachio.XIntegrationTests.Utils.Extractors;

public static class BaseExtractor
{
    private static readonly Lock _lock = new Lock();
    private static readonly SqlDbHelper _dbHelper = BaseControllerTests.GetSqlDbHelper();

    public static TEntity Extract<TEntity>(
        TEntity entity,
        Func<TEntity, bool> searchPredicate,
        bool hasIdentity = true)
        where TEntity : class, new()
    {
        lock (_lock)
        {
            if (_dbHelper.Any(searchPredicate))
            {
                return _dbHelper.GetItemWithPredicate(searchPredicate)!;
            }

            if (hasIdentity)
            {
                _dbHelper.AddItemWithCustomId(entity);
            }
            else
            {
                _dbHelper.AddItem(entity);
                _dbHelper.SaveChanges();
            }

            return _dbHelper.GetItemWithPredicate(searchPredicate)!;
        }
    }

    public static void RemoveByPredicate<T>(Func<T, bool> searchPredicate)
        where T : class, new()
    {
        lock (_lock)
        {
            var entity = _dbHelper.GetItemWithPredicate(searchPredicate);

            if (entity is null)
            {
                return;
            }

            _dbHelper.DeleteItem(entity);
            _dbHelper.SaveChanges();
        }
    }

    public static void RemoveById<TEntity>(Guid id)
        where TEntity : class, new()
    {
        lock (_lock)
        {
            var entity = _dbHelper.GetItemWithId<TEntity>(id);

            if (entity is null)
            {
                return;
            }

            _dbHelper.DeleteItem(entity);
            _dbHelper.SaveChanges();
        }
    }
}
