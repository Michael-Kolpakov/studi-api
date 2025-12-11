namespace Teachio.BLL.Services.Interfaces;

public interface IEntityExistenceService
{
    Task<(bool Exists, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull;

    Task<(bool Exists, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull;

    Task<(bool Exists, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull;

    Task<(bool Exists, string? ErrorMessage)> CheckUserExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull;
}
