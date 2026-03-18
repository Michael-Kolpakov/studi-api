using AutoMapper;
using Moq;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.MediatR.Courses.Courses.Create;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.Mocks.Localizers;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

/// <summary>
/// Represents the <see cref="CreateCourseHandlerTests"/> type.
/// </summary>
public class CreateCourseHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly CannotMapLocalizerMock _cannotMapLocalizerMock;
    private readonly AlreadyExistsLocalizerMock _alreadyExistsLocalizerMock;

    private readonly CreateCourseHandler _sut;

    public CreateCourseHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _cannotMapLocalizerMock = new CannotMapLocalizerMock();
        _alreadyExistsLocalizerMock = new AlreadyExistsLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new CreateCourseHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _cannotMapLocalizerMock,
            _alreadyExistsLocalizerMock);
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    [Fact]
    public async Task Handle_WhenCourseCreateRequestDtoIsNull_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetCreateCourseCommand(isDtoNull: true);
        var expectedError = _cannotMapLocalizerMock["CannotMapNullToCourse"].Value;

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        MapperVerifications.VerifyMapWasCalled<CourseCreateRequestDto, CourseEntity>(_mockMapper, request.CourseCreateRequestDto);
        LoggerVerifications.VerifyLoggerErrorWasCalled(_mockLoggerService, request, expectedError);
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    [Fact]
    public async Task Handle_WhenCourseAlreadyExists_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetCreateCourseCommand();
        var courseCreateRequestDto = request.CourseCreateRequestDto;
        var ownerId = request.OwnerUserId;

        var course = CourseTestData.GetCourse(
            ownerId: ownerId,
            title: courseCreateRequestDto.Title,
            description: courseCreateRequestDto.Description!,
            thumbnailName: courseCreateRequestDto.ThumbnailName,
            sectionsCount: 0,
            videosCount: 0);

        var expectedError = _alreadyExistsLocalizerMock[
            "CourseAlreadyExists",
            course.CourseName,
            course.OwnerUserId].Value;

        MapperMocks.MockMap(_mockMapper, courseCreateRequestDto, course);
        RepositoryMocks.SetupGetFirstOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, courseCreateRequestDto, course);
        RepositoryVerifications.VarifyGetFirstOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        LoggerVerifications.VerifyLoggerErrorWasCalled(_mockLoggerService, request, expectedError);
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    [Fact]
    public async Task Handle_WhenCourseIsValid_ShouldCreateCourse()
    {
        // Arrange
        var request = GetCreateCourseCommand();
        var courseCreateRequestDto = request.CourseCreateRequestDto;

        var course = CourseTestData.GetCourse(
            title: courseCreateRequestDto.Title,
            description: courseCreateRequestDto.Description!,
            thumbnailName: courseCreateRequestDto.ThumbnailName,
            sectionsCount: 0,
            videosCount: 0);

        var courseResponseDto = CourseTestData.GetCourseResponseDto(
            courseCreateRequestDto.Title,
            courseCreateRequestDto.Description!);

        MapperMocks.MockMap(_mockMapper, courseCreateRequestDto, course);
        RepositoryMocks.SetupGetFirstOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, null);
        MapperMocks.MockMap(_mockMapper, course, courseResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, courseCreateRequestDto, course);
        RepositoryVerifications.VarifyGetFirstOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        RepositoryVerifications.VerifyCreateAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        RepositoryVerifications.VerifySaveChangesAsyncWasCalled(_mockRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, courseResponseDto);
    }

    #region Helper Methods

    private static CreateCourseCommand GetCreateCourseCommand(
        string title = "Title of a Course",
        string description = "Description fo a Course",
        string thumbnailName = "title-of-a-course.png",
        bool isDtoNull = false)
    {
        var courseCreateRequestDto = isDtoNull
            ? null
            : new CourseCreateRequestDto()
            {
                Title = title,
                Description = description,
                ThumbnailName = thumbnailName
            };

        var ownerUserId = Guid.NewGuid();

        return new CreateCourseCommand(courseCreateRequestDto!, ownerUserId);
    }

    #endregion
}
