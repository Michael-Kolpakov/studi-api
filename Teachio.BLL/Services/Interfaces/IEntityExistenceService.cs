namespace Teachio.BLL.Services.Interfaces;

public interface IEntityExistenceService
{
    Task<bool> CheckCourseExistenceAsync(Guid courseId, object request);

    Task<bool> CheckSectionExistenceAsync(Guid sectionId, object request);

    Task<bool> CheckVideoExistenceAsync(Guid videoId, object request);

    Task<bool> CheckUserExistenceAsync(Guid userId, object request);
}
