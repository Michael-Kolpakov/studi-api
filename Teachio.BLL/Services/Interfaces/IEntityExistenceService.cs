using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Services.Interfaces;

/// <summary>
/// Defines the contract for <see cref="IEntityExistenceService"/>.
/// </summary>
public interface IEntityExistenceService
{
    /// <summary>
    /// Checks whether a course exists by the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key value.</typeparam>
    /// <param name="key">The key value used to look up the course.</param>
    /// <param name="keyName">The name of the course property represented by <paramref name="key"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the found course and an error message when the course is not found.</returns>
    Task<(Course? Entity, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;

    /// <summary>
    /// Checks whether a section exists by the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key value.</typeparam>
    /// <param name="key">The key value used to look up the section.</param>
    /// <param name="keyName">The name of the section property represented by <paramref name="key"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the found section and an error message when the section is not found.</returns>
    Task<(Section? Entity, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;

    /// <summary>
    /// Checks whether a video exists by the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key value.</typeparam>
    /// <param name="key">The key value used to look up the video.</param>
    /// <param name="keyName">The name of the video property represented by <paramref name="key"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the found video and an error message when the video is not found.</returns>
    Task<(Video? Entity, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull;
}
