using System.Reflection;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Utils.Helpers;

public static class OrderIndexShiftHelper
{
    public static async Task ShiftOrderIndexesForCreateAsync(
        object repository,
        string tableName,
        string entityIdColumnName,
        Guid entityId,
        int targetOrderIndex,
        CancellationToken cancellationToken)
    {
        var table = $"[{DatabaseConstants.CoursesSchema}].[{tableName}]";
        const string orderIndexColumnName = "OrderIndex";

        var sql = $"""
            UPDATE {table}
            SET {orderIndexColumnName} = {orderIndexColumnName} + 1
            WHERE {entityIdColumnName} = '{entityId}'
                AND {orderIndexColumnName} >= {targetOrderIndex}
        """;

        await ExecuteSqlRawAsync(repository, sql, cancellationToken);
    }

    public static async Task ShiftOrderIndexesForUpdateAsync(
        object repository,
        string tableName,
        string entityIdColumnName,
        Guid entityId,
        string entityPrimaryKeyColumnName,
        Guid entityPrimaryKeyValue,
        int currentOrderIndex,
        int targetOrderIndex,
        CancellationToken cancellationToken)
    {
        const string orderIndexColumnName = "OrderIndex";
        var table = $"[{DatabaseConstants.CoursesSchema}].[{tableName}]";

        var sql = $"""
            UPDATE {table}
            SET {orderIndexColumnName} =
                CASE
                    WHEN {entityPrimaryKeyColumnName} = '{entityPrimaryKeyValue}'
                        THEN {targetOrderIndex}
                    WHEN {targetOrderIndex} < {currentOrderIndex}
                        AND {orderIndexColumnName} >= {targetOrderIndex}
                        AND {orderIndexColumnName} < {currentOrderIndex}
                        THEN {orderIndexColumnName} + 1
                    WHEN {targetOrderIndex} > {currentOrderIndex}
                        AND {orderIndexColumnName} <= {targetOrderIndex}
                        AND {orderIndexColumnName} > {currentOrderIndex}
                        THEN {orderIndexColumnName} - 1
                    ELSE {orderIndexColumnName}
                END
            WHERE {entityIdColumnName} = '{entityId}'
                AND (
                    {entityPrimaryKeyColumnName} = '{entityPrimaryKeyValue}'
                    OR (
                        {targetOrderIndex} < {currentOrderIndex}
                        AND {orderIndexColumnName} >= {targetOrderIndex}
                        AND {orderIndexColumnName} < {currentOrderIndex}
                    )
                    OR (
                        {targetOrderIndex} > {currentOrderIndex}
                        AND {orderIndexColumnName} <= {targetOrderIndex}
                        AND {orderIndexColumnName} > {currentOrderIndex}
                    )
                )
        """;

        await ExecuteSqlRawAsync(repository, sql, cancellationToken);
    }

    public static async Task ShiftOrderIndexesForDeleteAsync(
        object repository,
        string tableName,
        string entityIdColumnName,
        Guid entityId,
        int deletedOrderIndex,
        CancellationToken cancellationToken)
    {
        const string orderIndexColumnName = "OrderIndex";
        var table = $"[{DatabaseConstants.CoursesSchema}].[{tableName}]";

        var sql = $"""
            UPDATE {table}
            SET {orderIndexColumnName} = {orderIndexColumnName} - 1
            WHERE {entityIdColumnName} = '{entityId}'
                AND {orderIndexColumnName} > {deletedOrderIndex}
        """;

        await ExecuteSqlRawAsync(repository, sql, cancellationToken);
    }

    private static async Task ExecuteSqlRawAsync(object repository, string sql, CancellationToken cancellationToken)
    {
        var method = repository.GetType().GetMethod(
            "ExecuteSqlRaw",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [typeof(string), typeof(CancellationToken)],
            null);

        ArgumentNullException.ThrowIfNull(method, nameof(method));

        var result = method.Invoke(repository, [sql, cancellationToken]);

        if (result is Task task)
        {
            await task;
        }
    }
}
