# Public API Inventory

Generated: 2026-03-18

This file documents all current public interfaces (classes, interfaces, records, enums, structs, delegates, and public methods) in production projects: `Teachio.BLL`, `Teachio.DAL`, `Teachio.WebApi`.

## Full List

```text
Teachio.BLL\Dto\Courses\Courses\Request\Create\CourseCreateRequestDto.cs:3: public class CourseCreateRequestDto : CourseCreateUpdateRequestDto;
Teachio.BLL\Dto\Courses\Courses\Request\Create\ThumbnailUploadRequestDto.cs:6: public class ThumbnailUploadRequestDto
Teachio.BLL\Dto\Courses\Courses\Request\Update\CourseUpdateRequestDto.cs:5: public class CourseUpdateRequestDto : CourseCreateUpdateRequestDto
Teachio.BLL\Dto\Courses\Courses\Response\CoursePreviewResponseDto.cs:5: public class CoursePreviewResponseDto
Teachio.BLL\Dto\Courses\Courses\Response\CoursePreviewShortResponseDto.cs:3: public class CoursePreviewShortResponseDto
Teachio.BLL\Dto\Courses\Courses\Response\CourseResponseDto.cs:6: public class CourseResponseDto
Teachio.BLL\Dto\Courses\Courses\Response\PaginatedCoursesResponseDto.cs:3: public class PaginatedCoursesResponseDto
Teachio.BLL\Dto\Courses\Courses\Response\ThumbnailUploadResponseDto.cs:3: public class ThumbnailUploadResponseDto
Teachio.BLL\Dto\Courses\Sections\Request\Create\SectionCreateRequestDto.cs:5: public class SectionCreateRequestDto : SectionCreateUpdateRequestDto
Teachio.BLL\Dto\Courses\Sections\Request\Update\SectionUpdateRequestDto.cs:5: public class SectionUpdateRequestDto : SectionCreateUpdateRequestDto
Teachio.BLL\Dto\Courses\Sections\Response\SectionPreviewResponseDto.cs:5: public class SectionPreviewResponseDto
Teachio.BLL\Dto\Courses\Sections\Response\SectionResponseDto.cs:5: public class SectionResponseDto
Teachio.BLL\Dto\Courses\Sections\Response\SectionShortResponseDto.cs:5: public class SectionShortResponseDto
Teachio.BLL\Dto\Courses\Videos\VideoProgress\Request\Update\VideoProgressUpdateRequestDto.cs:6: public class VideoProgressUpdateRequestDto
Teachio.BLL\Dto\Courses\Videos\VideoProgress\Response\VideoProgressResponseDto.cs:3: public class VideoProgressResponseDto
Teachio.BLL\Dto\Courses\Videos\Videos\Request\Create\VideoCreateRequestDto.cs:5: public class VideoCreateRequestDto : VideoCreateUpdateRequestDto
Teachio.BLL\Dto\Courses\Videos\Videos\Request\Update\VideoUpdateRequestDto.cs:5: public class VideoUpdateRequestDto : VideoCreateUpdateRequestDto
Teachio.BLL\Dto\Courses\Videos\Videos\Response\VideoPreviewResponseDto.cs:3: public class VideoPreviewResponseDto
Teachio.BLL\Dto\Courses\Videos\Videos\Response\VideoResponseDto.cs:5: public class VideoResponseDto
Teachio.BLL\Dto\Courses\Videos\Videos\Response\VideoShortResponseDto.cs:5: public class VideoShortResponseDto
Teachio.BLL\Dto\Shared\ErrorDto.cs:3: public class ErrorDto
Teachio.BLL\Dto\Users\Request\Create\AppUserCreateRequestDto.cs:3: public class AppUserCreateRequestDto : AppUserCreateUpdateRequestDto;
Teachio.BLL\Dto\Users\Request\Update\AppUserUpdateRequestDto.cs:5: public class AppUserUpdateRequestDto : AppUserCreateUpdateRequestDto
Teachio.BLL\Dto\Users\Response\AppUserResponseDto.cs:5: public class AppUserResponseDto
Teachio.BLL\Mapping\Course\Course\CourseProfile.cs:10: public class CourseProfile : Profile
Teachio.BLL\Mapping\Course\Course\CourseProfile.cs:12: public CourseProfile()
Teachio.BLL\Mapping\Course\Section\SectionProfile.cs:10: public class SectionProfile : Profile
Teachio.BLL\Mapping\Course\Section\SectionProfile.cs:12: public SectionProfile()
Teachio.BLL\Mapping\Course\Video\Video\VideoProfile.cs:10: public class VideoProfile : Profile
Teachio.BLL\Mapping\Course\Video\Video\VideoProfile.cs:12: public VideoProfile()
Teachio.BLL\Mapping\Course\Video\VideoProgress\VideoProgressProfile.cs:8: public class VideoProgressProfile : Profile
Teachio.BLL\Mapping\Course\Video\VideoProgress\VideoProgressProfile.cs:10: public VideoProgressProfile()
Teachio.BLL\Mapping\User\AppUserProfile.cs:9: public class AppUserProfile : Profile
Teachio.BLL\Mapping\User\AppUserProfile.cs:11: public AppUserProfile()
Teachio.BLL\MediatR\Courses\Courses\Create\CreateCourseCommand.cs:8: public record CreateCourseCommand(CourseCreateRequestDto CourseCreateRequestDto, Guid OwnerUserId)
Teachio.BLL\MediatR\Courses\Courses\Create\CreateCourseHandler.cs:14: public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CourseResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\Create\CreateCourseHandler.cs:22: public CreateCourseHandler(
Teachio.BLL\MediatR\Courses\Courses\Create\CreateCourseHandler.cs:36: public async Task<Result<CourseResponseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\Delete\DeleteCourseCommand.cs:7: public record DeleteCourseCommand(Guid CourseId, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Courses\Delete\DeleteCourseHandler.cs:17: public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\Delete\DeleteCourseHandler.cs:25: public DeleteCourseHandler(
Teachio.BLL\MediatR\Courses\Courses\Delete\DeleteCourseHandler.cs:39: public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\GetById\GetCourseByIdHandler.cs:18: public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\GetById\GetCourseByIdHandler.cs:25: public GetCourseByIdHandler(
Teachio.BLL\MediatR\Courses\Courses\GetById\GetCourseByIdHandler.cs:37: public async Task<Result<CourseResponseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\GetById\GetCourseByIdQuery.cs:7: public record GetCourseByIdQuery(Guid CourseId, Guid RequestingUserId, Guid? SelectedVideoId = null)
Teachio.BLL\MediatR\Courses\Courses\GetByIdPreview\GetCoursePreviewByIdHandler.cs:17: public class GetCoursePreviewByIdHandler : IRequestHandler<GetCoursePreviewByIdQuery, Result<CoursePreviewResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\GetByIdPreview\GetCoursePreviewByIdHandler.cs:24: public GetCoursePreviewByIdHandler(
Teachio.BLL\MediatR\Courses\Courses\GetByIdPreview\GetCoursePreviewByIdHandler.cs:36: public async Task<Result<CoursePreviewResponseDto>> Handle(GetCoursePreviewByIdQuery request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\GetByIdPreview\GetCoursePreviewByIdQuery.cs:7: public record GetCoursePreviewByIdQuery(Guid CourseId)
Teachio.BLL\MediatR\Courses\Courses\GetPaginated\GetPaginatedCoursesHandler.cs:14: public class GetPaginatedCoursesHandler : IRequestHandler<GetPaginatedCoursesQuery, Result<PaginatedCoursesResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\GetPaginated\GetPaginatedCoursesHandler.cs:20: public GetPaginatedCoursesHandler(
Teachio.BLL\MediatR\Courses\Courses\GetPaginated\GetPaginatedCoursesHandler.cs:30: public async Task<Result<PaginatedCoursesResponseDto>> Handle(GetPaginatedCoursesQuery request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\GetPaginated\GetPaginatedCoursesQuery.cs:7: public record GetPaginatedCoursesQuery(ushort PageNumber, ushort PageSize, Guid? RequestingUserId)
Teachio.BLL\MediatR\Courses\Courses\Update\UpdateCourseCommand.cs:8: public record UpdateCourseCommand(CourseUpdateRequestDto CourseUpdateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Courses\Update\UpdateCourseHandler.cs:14: public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\Update\UpdateCourseHandler.cs:23: public UpdateCourseHandler(
Teachio.BLL\MediatR\Courses\Courses\Update\UpdateCourseHandler.cs:39: public async Task<Result<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Courses\UploadThumbnail\UploadThumbnailCommand.cs:8: public record UploadThumbnailCommand(ThumbnailUploadRequestDto ThumbnailUploadRequestDto)
Teachio.BLL\MediatR\Courses\Courses\UploadThumbnail\UploadThumbnailHandler.cs:9: public class UploadThumbnailHandler : IRequestHandler<UploadThumbnailCommand, Result<ThumbnailUploadResponseDto>>
Teachio.BLL\MediatR\Courses\Courses\UploadThumbnail\UploadThumbnailHandler.cs:22: public UploadThumbnailHandler(ILoggerService logger)
Teachio.BLL\MediatR\Courses\Courses\UploadThumbnail\UploadThumbnailHandler.cs:28: public async Task<Result<ThumbnailUploadResponseDto>> Handle(UploadThumbnailCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Sections\Create\CreateSectionCommand.cs:8: public record CreateSectionCommand(SectionCreateRequestDto SectionCreateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Sections\Create\CreateSectionHandler.cs:14: public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponseDto>>
Teachio.BLL\MediatR\Courses\Sections\Create\CreateSectionHandler.cs:24: public CreateSectionHandler(
Teachio.BLL\MediatR\Courses\Sections\Create\CreateSectionHandler.cs:42: public async Task<Result<SectionResponseDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Sections\Delete\DeleteSectionCommand.cs:7: public record DeleteSectionCommand(Guid SectionId, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Sections\Delete\DeleteSectionHandler.cs:17: public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, Result<SectionResponseDto>>
Teachio.BLL\MediatR\Courses\Sections\Delete\DeleteSectionHandler.cs:25: public DeleteSectionHandler(
Teachio.BLL\MediatR\Courses\Sections\Delete\DeleteSectionHandler.cs:39: public async Task<Result<SectionResponseDto>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Sections\GetById\GetSectionByIdHandler.cs:17: public class GetSectionByIdHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionResponseDto>>
Teachio.BLL\MediatR\Courses\Sections\GetById\GetSectionByIdHandler.cs:24: public GetSectionByIdHandler(
Teachio.BLL\MediatR\Courses\Sections\GetById\GetSectionByIdHandler.cs:36: public async Task<Result<SectionResponseDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Sections\GetById\GetSectionByIdQuery.cs:7: public record GetSectionByIdQuery(Guid SectionId, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Sections\Update\UpdateSectionCommand.cs:8: public record UpdateSectionCommand(SectionUpdateRequestDto SectionUpdateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Sections\Update\UpdateSectionHandler.cs:13: public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
Teachio.BLL\MediatR\Courses\Sections\Update\UpdateSectionHandler.cs:22: public UpdateSectionHandler(
Teachio.BLL\MediatR\Courses\Sections\Update\UpdateSectionHandler.cs:38: public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Videos\VideoProgress\Update\UpdateVideoProgressCommand.cs:8: public record UpdateVideoProgressCommand(VideoProgressUpdateRequestDto VideoProgressUpdateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Videos\VideoProgress\Update\UpdateVideoProgressHandler.cs:13: public class UpdateVideoProgressHandler : IRequestHandler<UpdateVideoProgressCommand, Result<VideoProgressResponseDto>>
Teachio.BLL\MediatR\Courses\Videos\VideoProgress\Update\UpdateVideoProgressHandler.cs:21: public UpdateVideoProgressHandler(
Teachio.BLL\MediatR\Courses\Videos\VideoProgress\Update\UpdateVideoProgressHandler.cs:35: public async Task<Result<VideoProgressResponseDto>> Handle(UpdateVideoProgressCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Videos\Videos\Create\CreateVideoCommand.cs:8: public record CreateVideoCommand(VideoCreateRequestDto VideoCreateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Videos\Videos\Create\CreateVideoHandler.cs:15: public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<VideoResponseDto>>
Teachio.BLL\MediatR\Courses\Videos\Videos\Create\CreateVideoHandler.cs:25: public CreateVideoHandler(
Teachio.BLL\MediatR\Courses\Videos\Videos\Create\CreateVideoHandler.cs:43: public async Task<Result<VideoResponseDto>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Videos\Videos\Delete\DeleteVideoCommand.cs:7: public record DeleteVideoCommand(Guid VideoId, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Videos\Videos\Delete\DeleteVideoHandler.cs:17: public class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result<VideoResponseDto>>
Teachio.BLL\MediatR\Courses\Videos\Videos\Delete\DeleteVideoHandler.cs:25: public DeleteVideoHandler(
Teachio.BLL\MediatR\Courses\Videos\Videos\Delete\DeleteVideoHandler.cs:39: public async Task<Result<VideoResponseDto>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Videos\Videos\GetById\GetVideoByIdHandler.cs:17: public class GetVideoByIdHandler : IRequestHandler<GetVideoByIdQuery, Result<VideoResponseDto>>
Teachio.BLL\MediatR\Courses\Videos\Videos\GetById\GetVideoByIdHandler.cs:24: public GetVideoByIdHandler(
Teachio.BLL\MediatR\Courses\Videos\Videos\GetById\GetVideoByIdHandler.cs:36: public async Task<Result<VideoResponseDto>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\Courses\Videos\Videos\GetById\GetVideoByIdQuery.cs:7: public record GetVideoByIdQuery(Guid VideoId, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Videos\Videos\Update\UpdateVideoCommand.cs:8: public record UpdateVideoCommand(VideoUpdateRequestDto VideoUpdateRequestDto, Guid RequestingUserId)
Teachio.BLL\MediatR\Courses\Videos\Videos\Update\UpdateVideoHandler.cs:13: public class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result<VideoResponseDto>>
Teachio.BLL\MediatR\Courses\Videos\Videos\Update\UpdateVideoHandler.cs:22: public UpdateVideoHandler(
Teachio.BLL\MediatR\Courses\Videos\Videos\Update\UpdateVideoHandler.cs:38: public async Task<Result<VideoResponseDto>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
Teachio.BLL\MediatR\ResultValidations\NullResult.cs:5: public class NullResult<T> : Result<T>
Teachio.BLL\MediatR\ResultValidations\NullResult.cs:7: public NullResult()
Teachio.BLL\Models\Email\Base\MessageData.cs:9: public abstract MimeMessage ToMimeMessage();
Teachio.BLL\Services\Interfaces\IEmailService.cs:5: public interface IEmailService
Teachio.BLL\Services\Interfaces\IEntityExistenceService.cs:7: public interface IEntityExistenceService
Teachio.BLL\Services\Interfaces\IGoogleService.cs:5: public interface IGoogleService
Teachio.BLL\Services\Interfaces\ILoggerService.cs:3: public interface ILoggerService
Teachio.BLL\Services\Realizations\EntityExistenceService.cs:14: public class EntityExistenceService : IEntityExistenceService
Teachio.BLL\Services\Realizations\EntityExistenceService.cs:19: public EntityExistenceService(
Teachio.BLL\Services\Realizations\EntityExistenceService.cs:27: public async Task<(Course? Entity, string? ErrorMessage)> CheckCourseExistenceAsync<TKey>(
Teachio.BLL\Services\Realizations\EntityExistenceService.cs:42: public async Task<(Section? Entity, string? ErrorMessage)> CheckSectionExistenceAsync<TKey>(
Teachio.BLL\Services\Realizations\EntityExistenceService.cs:60: public async Task<(Video? Entity, string? ErrorMessage)> CheckVideoExistenceAsync<TKey>(
Teachio.BLL\Services\Realizations\LoggerService.cs:6: public class LoggerService : ILoggerService
Teachio.BLL\Services\Realizations\LoggerService.cs:10: public LoggerService(ILogger logger)
Teachio.BLL\Services\Realizations\LoggerService.cs:15: public void LogInformation(string message)
Teachio.BLL\Services\Realizations\LoggerService.cs:20: public void LogWarning(string message)
Teachio.BLL\Services\Realizations\LoggerService.cs:25: public void LogDebug(string message)
Teachio.BLL\Services\Realizations\LoggerService.cs:30: public void LogError(object? request, string errorMessage, string? stackTrace = null)
Teachio.BLL\SharedResource\AlreadyExistsSharedResource.cs:3: public class AlreadyExistsSharedResource;
Teachio.BLL\SharedResource\CannotFindSharedResource.cs:3: public class CannotFindSharedResource;
Teachio.BLL\SharedResource\CannotMapSharedResource.cs:3: public class CannotMapSharedResource;
Teachio.BLL\SharedResource\NoPermissionsSharedResource.cs:3: public class NoPermissionsSharedResource;
Teachio.BLL\Utils\MappingResolvers\NameFromTitleResolver.cs:11: public class NameFromTitleResolver : IValueResolver<object, object, string>
Teachio.BLL\Utils\MappingResolvers\NameFromTitleResolver.cs:13: public string Resolve(object source, object destination, string destMember, ResolutionContext context)
Teachio.BLL\Utils\MappingResolvers\NameFromTitleResolver.cs:28: public static string CreateNameFromTitle(string title) =>
Teachio.BLL\Utils\MappingResolvers\OwnerUserIdResolver.cs:6: public class OwnerUserIdResolver : IValueResolver<object, Course, Guid>
Teachio.BLL\Utils\MappingResolvers\OwnerUserIdResolver.cs:8: public Guid Resolve(object source, Course destination, Guid destMember, ResolutionContext context)
Teachio.BLL\Utils\MappingResolvers\RelativePathResolver.cs:10: public class RelativePathResolver : IValueResolver<object, object, string>
Teachio.BLL\Utils\MappingResolvers\RelativePathResolver.cs:12: public string Resolve(object source, object destination, string destMember, ResolutionContext context)
Teachio.BLL\Utils\MappingResolvers\TotalDurationResolver.cs:7: public class TotalDurationResolver : IValueResolver<Course, object, float>
Teachio.BLL\Utils\MappingResolvers\TotalDurationResolver.cs:9: public float Resolve(Course source, object destination, float destMember, ResolutionContext context)
Teachio.BLL\Utils\MappingResolvers\TrimmedTitleResolver.cs:11: public class TrimmedTitleResolver : IValueResolver<object, object, string>
Teachio.BLL\Utils\MappingResolvers\TrimmedTitleResolver.cs:13: public string Resolve(object source, object destination, string destMember, ResolutionContext context)
Teachio.BLL\Utils\MappingResolvers\VideosCountResolver.cs:7: public class VideosCountResolver : IValueResolver<Course, CoursePreviewShortResponseDto, int>
Teachio.BLL\Utils\MappingResolvers\VideosCountResolver.cs:9: public int Resolve(Course source, CoursePreviewShortResponseDto destination, int destMember, ResolutionContext context)
Teachio.DAL\Entities\Courses\Courses\Course.cs:6: public class Course
Teachio.DAL\Entities\Courses\Courses\CourseConfiguration.cs:12: public static void ConfigureCourses(this ModelBuilder builder)
Teachio.DAL\Entities\Courses\Sections\Section.cs:6: public class Section
Teachio.DAL\Entities\Courses\Sections\SectionConfiguration.cs:11: public static void ConfigureSections(this ModelBuilder builder)
Teachio.DAL\Entities\Courses\Videos\VideoProgress\VideoProgress.cs:5: public class VideoProgress
Teachio.DAL\Entities\Courses\Videos\VideoProgress\VideoProgressConfiguration.cs:10: public static void ConfigureVideoProgress(this ModelBuilder builder)
Teachio.DAL\Entities\Courses\Videos\Videos\Video.cs:5: public class Video
Teachio.DAL\Entities\Courses\Videos\Videos\Video.cs:32: public enum VideoStatus
Teachio.DAL\Entities\Courses\Videos\Videos\VideoConfiguration.cs:10: public static void ConfigureVideos(this ModelBuilder builder)
Teachio.DAL\Entities\Shared\UserCourse.cs:6: public class UserCourse
Teachio.DAL\Entities\Users\AppUser.cs:6: public class AppUser : IdentityUser<Guid>
Teachio.DAL\Entities\Users\AppUserConfiguration.cs:7: public static void ConfigureAppUsers(this ModelBuilder builder)
Teachio.DAL\Persistence\Seed\TeachioDbSeed.cs:22: public static async Task SeedAsync(
Teachio.DAL\Persistence\TeachioDbContext.cs:13: public class TeachioDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
Teachio.DAL\Persistence\TeachioDbContext.cs:15: public TeachioDbContext()
Teachio.DAL\Persistence\TeachioDbContext.cs:19: public TeachioDbContext(DbContextOptions<TeachioDbContext> options)
Teachio.DAL\Repositories\Interfaces\Base\IRepositoryBase.cs:8: public interface IRepositoryBase<T>
Teachio.DAL\Repositories\Interfaces\Base\IRepositoryWrapper.cs:9: public interface IRepositoryWrapper
Teachio.DAL\Repositories\Interfaces\Base\IRepositoryWrapper.cs:21: public int SaveChanges();
Teachio.DAL\Repositories\Interfaces\Base\IRepositoryWrapper.cs:23: public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
Teachio.DAL\Repositories\Interfaces\Courses\Courses\ICoursesRepository.cs:6: public interface ICoursesRepository : IRepositoryBase<Course>;
Teachio.DAL\Repositories\Interfaces\Courses\Sections\ISectionsRepository.cs:6: public interface ISectionsRepository : IRepositoryBase<Section>;
Teachio.DAL\Repositories\Interfaces\Courses\Videos\VideoProgress\IVideoProgressRepository.cs:5: public interface IVideoProgressRepository : IRepositoryBase<Entities.Courses.Videos.VideoProgress.VideoProgress>;
Teachio.DAL\Repositories\Interfaces\Courses\Videos\Videos\IVideosRepository.cs:6: public interface IVideosRepository : IRepositoryBase<Video>;
Teachio.DAL\Repositories\Interfaces\Users\IAppUsersRepository.cs:6: public interface IAppUsersRepository : IRepositoryBase<AppUser>;
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:21: public IQueryable<T> FindAll(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:28: public T Create(T entity)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:33: public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:40: public Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:45: public T Update(T entity)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:50: public void UpdateRange(IEnumerable<T> items)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:55: public T Delete(T entity)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:60: public void DeleteRange(IEnumerable<T> items)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:65: public void Attach(T entity)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:70: public EntityEntry<T> Entry(T entity)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:75: public Task ExecuteSqlRaw(string query, CancellationToken cancellationToken = default)
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:80: public async Task<IEnumerable<T>> GetAllAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:88: public async Task<PaginationResponse<T>> GetAllPaginatedAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:132: public async Task<T?> GetSingleOrDefaultAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:140: public async Task<T?> GetFirstOrDefaultAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:148: public async Task<T?> GetFirstOrDefaultAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:157: public async Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:172: public async Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:187: public async Task<List<TResult>> GetProjectedListAsync<TResult>(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:202: public async Task<T?> GetFirstOrDefaultAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:221: public async Task<int> GetSelfCountAsync(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:232: public async Task<int> GetNavigationCollectionsCountAsync<TProperty>(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:244: public async Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
Teachio.DAL\Repositories\Realizations\Base\RepositoryBase.cs:309: public ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
Teachio.DAL\Repositories\Realizations\Base\RepositoryWrapper.cs:16: public class RepositoryWrapper : IRepositoryWrapper
Teachio.DAL\Repositories\Realizations\Base\RepositoryWrapper.cs:30: public RepositoryWrapper(TeachioDbContext dbContext)
Teachio.DAL\Repositories\Realizations\Base\RepositoryWrapper.cs:50: public int SaveChanges()
Teachio.DAL\Repositories\Realizations\Base\RepositoryWrapper.cs:55: public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
Teachio.DAL\Repositories\Realizations\Courses\Courses\CoursesRepository.cs:8: public class CoursesRepository(TeachioDbContext dbContext)
Teachio.DAL\Repositories\Realizations\Courses\Sections\SectionsRepository.cs:8: public class SectionsRepository(TeachioDbContext dbContext)
Teachio.DAL\Repositories\Realizations\Courses\Videos\VideoProgress\VideoProgressRepository.cs:7: public class VideoProgressRepository(TeachioDbContext dbContext)
Teachio.DAL\Repositories\Realizations\Courses\Videos\Videos\VideosRepository.cs:8: public class VideosRepository(TeachioDbContext dbContext)
Teachio.DAL\Repositories\Realizations\Users\AppUsersRepository.cs:8: public class AppUsersRepository(TeachioDbContext dbContext)
Teachio.DAL\SharedResource\DataAnnotationsSharedResource.cs:3: public class DataAnnotationsSharedResource;
Teachio.DAL\Utils\Helpers\Constraint.cs:5: public class Constraint
Teachio.DAL\Utils\Helpers\Constraint.cs:7: public static string CreateSqlRegexCheck(string columnName, ValidationRule rule)
Teachio.DAL\Utils\Helpers\Constraint.cs:22: public static string CreateSqlNonNegativeCheck(string columnName)
Teachio.DAL\Utils\Helpers\Constraint.cs:27: public static string CreateSqlRangeCheck(string columnName, int minValue, int maxValue)
Teachio.DAL\Utils\Helpers\Constraint.cs:47: public enum ValidationRule
Teachio.DAL\Utils\Helpers\Constraint.cs:56: public enum CheckConstraintType
Teachio.DAL\Utils\Helpers\PaginationResponse.cs:3: public class PaginationResponse<T>
Teachio.DAL\Utils\Helpers\PaginationResponse.cs:28: public static PaginationResponse<T> Create(
Teachio.DAL\Utils\Validators\DateValidatorAttribute.cs:6: public class DateValidatorAttribute : ValidationAttribute
Teachio.DAL\Utils\Validators\DateValidatorAttribute.cs:12: public DateValidatorAttribute(string minDate, string maxDate)
Teachio.WebApi\Controllers\BaseApiController.cs:12: public class BaseApiController : ControllerBase
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:16: public class CoursesController : BaseApiController
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:27: public async Task<IActionResult> GetPaginated([FromQuery] ushort pageNumber, [FromQuery] ushort pageSize)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:44: public async Task<IActionResult> GetByIdPreview([FromRoute] Guid id)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:59: public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] Guid? selectedVideoId = null)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:78: public async Task<IActionResult> Create([FromBody] CourseCreateRequestDto courseCreateRequestDto)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:97: public async Task<IActionResult> UploadThumbnail([FromForm] ThumbnailUploadRequestDto thumbnailUploadRequestDto)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:112: public async Task<IActionResult> Update([FromBody] CourseUpdateRequestDto courseUpdateRequestDto)
Teachio.WebApi\Controllers\Courses\Courses\CoursesController.cs:132: public async Task<IActionResult> Delete([FromRoute] Guid id)
Teachio.WebApi\Controllers\Courses\Sections\SectionsController.cs:13: public class SectionsController : BaseApiController
Teachio.WebApi\Controllers\Courses\Sections\SectionsController.cs:24: public async Task<IActionResult> GetById([FromRoute] Guid id)
Teachio.WebApi\Controllers\Courses\Sections\SectionsController.cs:43: public async Task<IActionResult> Create([FromBody] SectionCreateRequestDto sectionCreateRequestDto)
Teachio.WebApi\Controllers\Courses\Sections\SectionsController.cs:62: public async Task<IActionResult> Update([FromBody] SectionUpdateRequestDto sectionUpdateRequestDto)
Teachio.WebApi\Controllers\Courses\Sections\SectionsController.cs:82: public async Task<IActionResult> Delete([FromRoute] Guid id)
Teachio.WebApi\Controllers\Courses\Videos\VideoProgress\VideosProgressController.cs:9: public class VideosProgressController : BaseApiController
Teachio.WebApi\Controllers\Courses\Videos\VideoProgress\VideosProgressController.cs:21: public async Task<IActionResult> Update([FromBody] VideoProgressUpdateRequestDto videoProgressUpdateRequestDto)
Teachio.WebApi\Controllers\Courses\Videos\Videos\VideosController.cs:13: public class VideosController : BaseApiController
Teachio.WebApi\Controllers\Courses\Videos\Videos\VideosController.cs:24: public async Task<IActionResult> GetById([FromRoute] Guid id)
Teachio.WebApi\Controllers\Courses\Videos\Videos\VideosController.cs:43: public async Task<IActionResult> Create([FromBody] VideoCreateRequestDto videoCreateRequestDto)
Teachio.WebApi\Controllers\Courses\Videos\Videos\VideosController.cs:62: public async Task<IActionResult> Update([FromBody] VideoUpdateRequestDto videoUpdateRequestDto)
Teachio.WebApi\Controllers\Courses\Videos\Videos\VideosController.cs:82: public async Task<IActionResult> Delete([FromRoute] Guid id)
Teachio.WebApi\Extensions\ApplicationServicesExtensions.cs:15: public static IServiceCollection AddApplicationServices(
Teachio.WebApi\Extensions\ConfigurationBuilderExtensions.cs:5: public static IConfigurationBuilder ConfigureCustom(this IConfigurationBuilder builder, string environment)
Teachio.WebApi\Extensions\ConfigureHostBuilderExtensions.cs:5: public static void ConfigureApplication(this ConfigureHostBuilder builder, WebApplicationBuilder appBuilder)
Teachio.WebApi\Extensions\DatabaseExtensions.cs:12: public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
Teachio.WebApi\Extensions\DatabaseExtensions.cs:33: public static async Task InitializeDatabase(IApplicationBuilder app, CancellationToken cancellationToken)
Teachio.WebApi\Extensions\SerilogExtensions.cs:12: public static void AddSerilogLogging(this IServiceCollection services)
Teachio.WebApi\Extensions\SwaggerExtensions.cs:8: public static IServiceCollection AddSwagger(this IServiceCollection services)
Teachio.WebApi\Extensions\SwaggerExtensions.cs:28: public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
Teachio.WebApi\Middlewares\ExceptionHandlingMiddleware.cs:8: public class ExceptionHandlingMiddleware
Teachio.WebApi\Middlewares\ExceptionHandlingMiddleware.cs:19: public ExceptionHandlingMiddleware(RequestDelegate next)
Teachio.WebApi\Middlewares\ExceptionHandlingMiddleware.cs:34: public async Task InvokeAsync(HttpContext context)
Teachio.WebApi\Program.cs:9: public class Program
Teachio.WebApi\Program.cs:11: public static async Task Main(string[] args)
```

## Maintenance Rules

- Any new public API must be added to this list in the same pull request.
- API changes (rename, delete, signature change) must update this file and mention migration impact in PR description.
- Prefer XML comments (`///`) on all public types and methods; keep summaries short and action-oriented.
- Breaking changes must include an entry in release notes.
