namespace Studi.BLL.Services.Interfaces;

public interface ICourseAccessService
{
    Task<bool> HasAccessToCourseAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasAccessToSectionAsync(Guid sectionId, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasAccessToVideoAsync(Guid videoId, Guid userId, CancellationToken cancellationToken = default);
}
