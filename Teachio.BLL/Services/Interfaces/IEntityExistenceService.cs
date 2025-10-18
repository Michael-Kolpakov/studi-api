namespace Teachio.BLL.Services.Interfaces;

public interface IEntityExistenceService
{
    Task<(bool Exists, string? ErrorMessage)> CheckCourseExistenceAsync(Guid courseId, object request);

    Task<(bool Exists, string? ErrorMessage)> CheckSectionExistenceAsync(Guid sectionId, object request);

    Task<(bool Exists, string? ErrorMessage)> CheckVideoExistenceAsync(Guid videoId, object request);

    Task<(bool Exists, string? ErrorMessage)> CheckUserExistenceAsync(Guid userId, object request);
}
