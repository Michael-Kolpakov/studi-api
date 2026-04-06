using AutoMapper;
using Moq;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.MediatR.Courses.Courses.Update;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.Mocks.Localizers;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

public class UpdateCourseHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
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
            _cannotFindLocalizerMock,
            _noPermissionsLocalizerMock,
            _alreadyExistsLocalizerMock);
    }

    [Fact]
    public async Task Handle_WhenCourseDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetUpdateCourseCommand();
        var expectedError = _cannotFindLocalizerMock["CannotFindCourseById", request.CourseUpdateRequestDto.Id].Value;

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
        var request = GetUpdateCourseCommand();
        var courseUpdateRequestDto = request.CourseUpdateRequestDto;

        var existingCourse = CourseTestData.GetCourse(
            ownerId: request.RequestingUserId,
            title: "Old Title",
            description: "Old Description",
            thumbnailName: "old-title.png",
            sectionsCount: 0,
            videosCount: 0);

        var updatedCourse = CourseTestData.GetCourse(
            courseId: courseUpdateRequestDto.Id,
            title: courseUpdateRequestDto.Title,
            description: courseUpdateRequestDto.Description!,
            thumbnailName: courseUpdateRequestDto.ThumbnailName,
            sectionsCount: 0,
            videosCount: 0);

        var courseResponseDto = CourseTestData.GetCourseResponseDto(
            courseUpdateRequestDto.Title,
            courseUpdateRequestDto.Description!);

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
        string description = "Updated Course Description",
        string thumbnailName = "updated-course-title.png")
    {
        var courseUpdateRequestDto = new CourseUpdateRequestDto()
        {
            Id = courseId ?? Guid.NewGuid(),
            Title = title,
            Description = description,
            ThumbnailName = thumbnailName
        };

        var requestingUserId = Guid.NewGuid();

        return new UpdateCourseCommand(courseUpdateRequestDto, requestingUserId);
    }

    #endregion
}
