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

    public Task<(bool Exists, string? ErrorMessage)> CheckCourseExistenceAsync(Guid courseId, object request)
        => CheckExistenceAsync(
            courseId,
            id => _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            nameof(Course),
            _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), courseId].Value);

    public Task<(bool Exists, string? ErrorMessage)> CheckSectionExistenceAsync(Guid sectionId, object request)
        => CheckExistenceAsync(
            sectionId,
            id => _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            nameof(Section),
            _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindSectionById), sectionId].Value);

    public Task<(bool Exists, string? ErrorMessage)> CheckVideoExistenceAsync(Guid videoId, object request)
        => CheckExistenceAsync(
            videoId,
            id => _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            nameof(Video),
            _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindVideoById), videoId].Value);

    public Task<(bool Exists, string? ErrorMessage)> CheckUserExistenceAsync(Guid userId, object request)
        => CheckExistenceAsync(
            userId,
            id => _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            nameof(AppUser),
            _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindUserById), userId].Value);

    private async Task<(bool Exists, string? ErrorMessage)> CheckExistenceAsync<T>(
        Guid id,
        Func<Guid, Task<T?>> fetchFunc,
        object? request,
        string entityName,
        string errorMessage)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(fetchFunc);

        var entity = await fetchFunc(id);
        if (entity is not null)
        {
            return (true, null);
        }

        var requestInfo = request ?? new
        {
            Service = nameof(EntityExistenceService),
            Entity = entityName
        };
        _logger.LogError(requestInfo, errorMessage);

        return (false, errorMessage);
    }
}
