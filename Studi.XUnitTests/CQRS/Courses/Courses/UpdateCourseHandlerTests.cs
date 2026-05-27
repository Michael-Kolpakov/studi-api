using AutoMapper;
using Moq;
using Studi.BLL.CQRS.Courses.Courses.Update;
using Studi.BLL.DTOs.Courses.Courses.Request.Update;
using Studi.BLL.Services.Interfaces;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Repositories.Interfaces.Courses.Courses;
using Studi.XUnitTests.Mocks;
using Studi.XUnitTests.Mocks.Localizers;
using Studi.XUnitTests.TestData;
using Studi.XUnitTests.Verifications;

namespace Studi.XUnitTests.CQRS.Courses.Courses;

public class UpdateCourseHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly CannotFindLocalizerMock _cannotFindLocalizerMock;
    private readonly NoPermissionsLocalizerMock _noPermissionsLocalizerMock;
    private readonly AlreadyExistsLocalizerMock _alreadyExistsLocalizerMock;

    private readonly UpdateCourseHandler _sut;

    public UpdateCourseHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _cannotFindLocalizerMock = new CannotFindLocalizerMock();
        _noPermissionsLocalizerMock = new NoPermissionsLocalizerMock();
        _alreadyExistsLocalizerMock = new AlreadyExistsLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new UpdateCourseHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _mockCurrentUserService.Object,
            _cannotFindLocalizerMock,
            _noPermissionsLocalizerMock,
            _alreadyExistsLocalizerMock);
    }

    [Fact]
    public async Task Handle_WhenCourseDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        var requestingUserId = Guid.NewGuid();
        var request = GetUpdateCourseCommand();
        var expectedError = _cannotFindLocalizerMock["CannotFindCourseById", request.CourseUpdateRequestDto.Id].Value;

        CurrentUserMocks.SetupGetUserIdMock(_mockCurrentUserService, requestingUserId);
        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        LoggerVerifications.VerifyLoggerErrorWasCalled(_mockLoggerService, request, expectedError);
    }

    [Fact]
    public async Task Handle_WhenCourseExists_ShouldUpdateCourse()
    {
        // Arrange
        var requestingUserId = Guid.NewGuid();
        var request = GetUpdateCourseCommand();
        var courseUpdateRequestDto = request.CourseUpdateRequestDto;

        var existingCourse = CourseTestData.GetCourse(
            ownerId: requestingUserId,
            title: "Old Title",
            description: "Old Description",
            sectionsCount: 0,
            videosCount: 0);

        var updatedCourse = CourseTestData.GetCourse(
            courseId: courseUpdateRequestDto.Id,
            title: courseUpdateRequestDto.Title,
            description: courseUpdateRequestDto.Description!,
            sectionsCount: 0,
            videosCount: 0);

        var courseResponseDto = CourseTestData.GetCourseResponseDto(
            courseUpdateRequestDto.Title,
            courseUpdateRequestDto.Description!);

        CurrentUserMocks.SetupGetUserIdMock(_mockCurrentUserService, requestingUserId);
        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, existingCourse);
        MapperMocks.MockMapToExisting(_mockMapper, courseUpdateRequestDto, existingCourse);
        RepositoryMocks.SetupUpdateMock(_mockRepository, wrapper => wrapper.CoursesRepository, updatedCourse);
        MapperMocks.MockMap(_mockMapper, existingCourse, courseResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapToExistingWasCalled(_mockMapper, courseUpdateRequestDto, existingCourse);
        RepositoryVerifications.VerifyUpdateWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        RepositoryVerifications.VerifySaveChangesAsyncWasCalled(_mockRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, existingCourse, courseResponseDto);
    }

    #region Helper Methods

    private static UpdateCourseCommand GetUpdateCourseCommand(
        Guid? courseId = null,
        string title = "Updated Course Title",
        string description = "Updated Course Description")
    {
        var courseUpdateRequestDto = new CourseUpdateRequestDto()
        {
            Id = courseId ?? Guid.NewGuid(),
            Title = title,
            Description = description
        };

        return new UpdateCourseCommand(courseUpdateRequestDto);
    }

    #endregion
}
