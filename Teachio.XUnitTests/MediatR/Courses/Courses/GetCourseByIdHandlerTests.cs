using AutoMapper;
using Moq;
using Teachio.BLL.MediatR.Courses.Courses.GetById;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.Mocks.Localizers;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

public class GetCourseByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly CannotFindLocalizerMock _cannotFindLocalizerMock;

    private readonly GetCourseByIdHandler _sut;

    public GetCourseByIdHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _cannotFindLocalizerMock = new CannotFindLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new GetCourseByIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _cannotFindLocalizerMock);
    }

    [Fact]
    public async Task Handle_WhenCourseDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetGetCourseByIdQuery();
        var expectedError = _cannotFindLocalizerMock["CannotFindCourseById", request.CourseId].Value;

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        LoggerVerifications.VerifyLoggerErrorWasCalled(_mockLoggerService, request, expectedError);
    }

    [Fact]
    public async Task Handle_WhenCourseExistsWithSelectedVideoId_ShouldReturnCourseWithSelectedVideo()
    {
        // Arrange
        var selectedVideoId = Guid.NewGuid();
        var request = GetGetCourseByIdQuery(selectedVideoId: selectedVideoId);
        var course = CourseTestData.GetCourse();

        var selectedVideo = course.Sections[1].Videos[0];
        selectedVideo.Id = selectedVideoId;

        var courseResponseDto = CourseTestData.GetCourseResponseDto(course);
        var videoResponseDto = VideoTestData.GetVideoResponseDto(selectedVideo);

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);
        MapperMocks.MockMap(_mockMapper, course, courseResponseDto);
        MapperMocks.MockMap(_mockMapper, selectedVideo, videoResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, courseResponseDto);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, selectedVideo, videoResponseDto);
    }

    [Fact]
    public async Task Handle_WhenCourseExistsWithoutSelectedVideoId_ShouldReturnCourseWithFirstIncompleteVideo()
    {
        // Arrange
        var request = GetGetCourseByIdQuery(selectedVideoId: null);
        var course = CourseTestData.GetCourse();

        course.Sections[0].Videos[0].VideoProgress.IsCompleted = true;
        var firstIncompleteVideo = course.Sections[0].Videos[1];
        firstIncompleteVideo.VideoProgress.IsCompleted = false;

        var courseResponseDto = CourseTestData.GetCourseResponseDto(course);
        var videoResponseDto = VideoTestData.GetVideoResponseDto(firstIncompleteVideo);

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);
        MapperMocks.MockMap(_mockMapper, course, courseResponseDto);
        MapperMocks.MockMap(_mockMapper, firstIncompleteVideo, videoResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, courseResponseDto);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, firstIncompleteVideo, videoResponseDto);
    }

    [Fact]
    public async Task Handle_WhenAllVideosAreCompleted_ShouldReturnCourseWithFirstVideoOfFirstSection()
    {
        // Arrange
        var request = GetGetCourseByIdQuery(selectedVideoId: null);
        var course = CourseTestData.GetCourse();

        foreach (var video in course.Sections.SelectMany(section => section.Videos))
        {
            video.VideoProgress.IsCompleted = true;
        }

        var firstVideo = course.Sections
            .OrderBy(s => s.OrderIndex)
            .First()
            .Videos
            .OrderBy(v => v.OrderIndex)
            .First();

        var courseResponseDto = CourseTestData.GetCourseResponseDto(course);
        var videoResponseDto = VideoTestData.GetVideoResponseDto(firstVideo);

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);
        MapperMocks.MockMap(_mockMapper, course, courseResponseDto);
        MapperMocks.MockMap(_mockMapper, firstVideo, videoResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, courseResponseDto);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, firstVideo, videoResponseDto);
    }

    #region Helper Methods

    private static GetCourseByIdQuery GetGetCourseByIdQuery(
        Guid? courseId = null,
        Guid? requestingUserId = null,
        Guid? selectedVideoId = null)
    {
        return new GetCourseByIdQuery(
            courseId ?? Guid.NewGuid(),
            requestingUserId ?? Guid.NewGuid(),
            selectedVideoId);
    }

    #endregion
}
