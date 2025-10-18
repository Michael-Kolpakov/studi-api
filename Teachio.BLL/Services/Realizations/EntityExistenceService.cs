using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.Services.Realizations;

public class EntityExistenceService : IEntityExistenceService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public EntityExistenceService(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CheckCourseExistenceAsync(Guid courseId, object request)
        => CheckExistenceAsync(
            courseId,
            id => _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            "course");

    public Task<bool> CheckSectionExistenceAsync(Guid sectionId, object request)
        => CheckExistenceAsync(
            sectionId,
            id => _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            "section");

    public Task<bool> CheckVideoExistenceAsync(Guid videoId, object request)
        => CheckExistenceAsync(
            videoId,
            id => _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(x => x.Id == id),
            request,
            "video");

    public Task<bool> CheckUserExistenceAsync(Guid userId, object request)
        => CheckExistenceAsync(
            userId,
            id => _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultAsync(x => x.Id == id.ToString()),
            request,
            "user");

    private async Task<bool> CheckExistenceAsync<T>(
        Guid id,
        Func<Guid, Task<T?>> fetchFunc,
        object? request,
        string entityName)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(fetchFunc);

        var entity = await fetchFunc(id);
        if (entity is not null)
        {
            return true;
        }

        var requestInfo = request ?? new
        {
            Service = nameof(EntityExistenceService),
            Entity = entityName
        };
        var errorMessage = $"There is no {entityName} with such Id: {id}";
        _logger.LogError(requestInfo, errorMessage);

        return false;
    }
}
