using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.Videos.Videos;

namespace Studi.BLL.Services.Interfaces;

public interface IEntityExistenceService
{
    Task<(Course? Entity, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;

    Task<(Section? Entity, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;

    Task<(Video? Entity, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;
}
