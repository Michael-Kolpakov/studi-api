using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.Services.Realizations;

public class EntityExistenceService : IEntityExistenceService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public EntityExistenceService(
        IRepositoryWrapper repositoryWrapper,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
        _stringLocalizerCannotFind = stringLocalizerCannotFind ?? throw new ArgumentNullException(nameof(stringLocalizerCannotFind));
    }

    public async Task<(Course? Entity, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        return await CheckExistenceWithErrorHandlingAsync<Course, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(expr, cancellationToken: cancellationToken),
            nameof(Course),
            nameof(CannotFindSharedResource_en.CannotFindCourseById),
            nameof(CannotFindSharedResource_en.CannotFindCourseByKey));
    }

    public async Task<(Section? Entity, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        return await CheckExistenceWithErrorHandlingAsync<Section, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
                expr,
                x => x.Include(s => s.Course!),
                cancellationToken),
            nameof(Section),
            nameof(CannotFindSharedResource_en.CannotFindSectionById),
            nameof(CannotFindSharedResource_en.CannotFindSectionByKey));
    }

    public async Task<(Video? Entity, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
        TKey key,
        string keyName,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        return await CheckExistenceWithErrorHandlingAsync<Video, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(expr, cancellationToken: cancellationToken),
            nameof(Video),
            nameof(CannotFindSharedResource_en.CannotFindVideoById),
            nameof(CannotFindSharedResource_en.CannotFindVideoByKey));
    }

    private async Task<(TEntity? Entity, string? ErrorMessage)> CheckExistenceWithErrorHandlingAsync<TEntity, TKey>(
        TKey key,
        string keyName,
        Func<Expression<Func<TEntity, bool>>, Task<TEntity?>> fetchFunc,
        string entityName,
        string byIdResourceName,
        string byKeyResourceName)
        where TEntity : class
        where TKey : notnull
    {
        var errorMessage = BuildErrorMessage(
            key,
            keyName,
            entityName,
            byIdResourceName,
            byKeyResourceName);

        try
        {
            var entity = await CheckExistenceAsync(key, keyName, fetchFunc);

            return entity is null ? (null, errorMessage) : (entity, null);
        }
        catch (ArgumentException)
        {
            var propertyNotFoundErrorMessage = $"Property '{keyName}' not found on type '{entityName}'.";

            return (null, propertyNotFoundErrorMessage);
        }
    }

    private string BuildErrorMessage<TKey>(
        TKey key,
        string keyName,
        string entityName,
        string byIdResourceName,
        string byKeyResourceName)
        where TKey : notnull
    {
        var expectedIdName = string.Concat(entityName, "Id");

        var isGuidKey = typeof(TKey) == typeof(Guid);
        var isIdName = string.Equals(keyName, "Id", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(keyName, expectedIdName, StringComparison.OrdinalIgnoreCase);

        return isGuidKey && isIdName
            ? _stringLocalizerCannotFind[byIdResourceName, key].Value
            : _stringLocalizerCannotFind[byKeyResourceName, key, keyName].Value;
    }

    private static async Task<TEntity?> CheckExistenceAsync<TEntity, TKey>(
        TKey key,
        string keyName,
        Func<Expression<Func<TEntity, bool>>, Task<TEntity?>> fetchFunc)
        where TEntity : class
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(fetchFunc);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyName);

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.PropertyOrField(parameter, keyName);
        var right = Expression.Constant(key, typeof(TKey));

        var body = Expression.Equal(property, right);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        var entity = await fetchFunc(lambda);

        return entity;
    }
}
