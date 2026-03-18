using AutoMapper;
using Moq;
using Teachio.BLL.MediatR.Courses.Courses.Delete;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.Mocks.Localizers;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

/// <summary>
/// Represents the <see cref="DeleteCourseHandlerTests"/> type.
/// </summary>
public class DeleteCourseHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly CannotFindLocalizerMock _cannotFindLocalizerMock;
    private readonly NoPermissionsLocalizerMock _noPermissionsLocalizerMock;

    private readonly DeleteCourseHandler _sut;

    public DeleteCourseHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _cannotFindLocalizerMock = new CannotFindLocalizerMock();
        _noPermissionsLocalizerMock = new NoPermissionsLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new DeleteCourseHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _cannotFindLocalizerMock,
            _noPermissionsLocalizerMock);
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    [Fact]
    public async Task Handle_WhenCourseDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetDeleteCourseCommand();
        var expectedError = _cannotFindLocalizerMock["CannotFindCourseById", request.CourseId].Value;

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        LoggerVerifications.VerifyLoggerErrorWasCalled(_mockLoggerService, request, expectedError);
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    [Fact]
    public async Task Handle_WhenCourseExists_ShouldDeleteCourse()
    {
        // Arrange
        var request = GetDeleteCourseCommand();

        var course = CourseTestData.GetCourse(
            courseId: request.CourseId,
            ownerId: request.RequestingUserId,
            title: "Title of a Course",
            description: "Description of a Course",
            thumbnailName: "title-of-a-course.png",
            sectionsCount: 0,
            videosCount: 0);

        var courseResponseDto = CourseTestData.GetCourseResponseDto(course.Title, course.Description!);

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);
        MapperMocks.MockMap(_mockMapper, course, courseResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        RepositoryVerifications.VerifyDeleteWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        RepositoryVerifications.VerifySaveChangesAsyncWasCalled(_mockRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, courseResponseDto);
    }

    #region Helper Methods

    private static DeleteCourseCommand GetDeleteCourseCommand(Guid? courseId = null, Guid? requestingUserId = null)
    {
        return new DeleteCourseCommand(
            courseId ?? Guid.NewGuid(),
            requestingUserId ?? Guid.NewGuid());
    }

    #endregion
}
