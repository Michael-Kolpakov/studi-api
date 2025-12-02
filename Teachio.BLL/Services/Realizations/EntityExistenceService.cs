using System.Linq.Expressions;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Entities.Users;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.Services.Realizations;

public class EntityExistenceService : IEntityExistenceService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public EntityExistenceService(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stringLocalizerCannotFind = stringLocalizerCannotFind ?? throw new ArgumentNullException(nameof(stringLocalizerCannotFind));
    }

    public Task<(bool Exists, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull
    {
        var errorMessage = BuildErrorMessage(
            key,
            keyName,
            nameof(Course),
            nameof(CannotFindSharedResource_en.CannotFindCourseById),
            nameof(CannotFindSharedResource_en.CannotFindCourseByKey));

        return CheckExistenceAsync<Course, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(expr),
            request,
            nameof(Course),
            errorMessage);
    }

    public Task<(bool Exists, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull
    {
        var errorMessage = BuildErrorMessage(
            key,
            keyName,
            nameof(Section),
            nameof(CannotFindSharedResource_en.CannotFindSectionById),
            nameof(CannotFindSharedResource_en.CannotFindSectionByKey));

        return CheckExistenceAsync<Section, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(expr),
            request,
            nameof(Section),
            errorMessage);
    }

    public Task<(bool Exists, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull
    {
        var errorMessage = BuildErrorMessage(
            key,
            keyName,
            nameof(Video),
            nameof(CannotFindSharedResource_en.CannotFindVideoById),
            nameof(CannotFindSharedResource_en.CannotFindVideoByKey));

        return CheckExistenceAsync<Video, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(expr),
            request,
            nameof(Video),
            errorMessage);
    }

    public Task<(bool Exists, string? ErrorMessage)> CheckUserExistenceAsync<TKey>(
        TKey key,
        string keyName,
        object request)
        where TKey : notnull
    {
        var errorMessage = BuildErrorMessage(
            key,
            keyName,
            nameof(AppUser),
            nameof(CannotFindSharedResource_en.CannotFindUserById),
            nameof(CannotFindSharedResource_en.CannotFindUserByKey));

        return CheckExistenceAsync<AppUser, TKey>(
            key,
            keyName,
            expr => _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultAsync(expr),
            request,
            nameof(AppUser),
            errorMessage);
    }

    private async Task<(bool Exists, string? ErrorMessage)> CheckExistenceAsync<TEntity, TKey>(
        TKey key,
        string keyName,
        Func<Expression<Func<TEntity, bool>>, Task<TEntity?>> fetchFunc,
        object? request,
        string entityName,
        string errorMessage)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(fetchFunc, nameof(fetchFunc));
        ArgumentException.ThrowIfNullOrWhiteSpace(keyName, nameof(keyName));

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? property;
        try
        {
            property = Expression.PropertyOrField(parameter, keyName);
        }
        catch (ArgumentException)
        {
            var propertyNotFoundMessage = $"Property '{keyName}' not found on type '{typeof(TEntity).Name}'.";
            var requestInfo = request ?? new
            {
                Service = nameof(EntityExistenceService),
                Entity = entityName,
                Key = keyName
            };
            _logger.LogError(requestInfo, propertyNotFoundMessage);

            return (false, propertyNotFoundMessage);
        }

        var constant = Expression.Constant(key, typeof(TKey));
        Expression right = constant;

        var body = Expression.Equal(property, right);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        var entity = await fetchFunc(lambda);
        if (entity is not null)
        {
            return (true, null);
        }

        var requestInfoNotFound = request ?? new
        {
            Service = nameof(EntityExistenceService),
            Entity = entityName,
            Key = keyName,
            Value = key
        };
        _logger.LogError(requestInfoNotFound, errorMessage);

        return (false, errorMessage);
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
}
