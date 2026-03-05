using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Services.Interfaces;

public interface IEntityExistenceService
{
    Task<(Course? Entity, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(TKey key, string keyName)
        where TKey : notnull;

    Task<(Section? Entity, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(TKey key, string keyName)
        where TKey : notnull;

    Task<(Video? Entity, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(TKey key, string keyName)
        where TKey : notnull;
}
