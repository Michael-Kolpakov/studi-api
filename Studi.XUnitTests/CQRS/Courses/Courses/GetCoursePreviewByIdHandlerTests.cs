using AutoMapper;
using Moq;
using Studi.BLL.CQRS.Courses.Courses.GetByIdPreview;
using Studi.BLL.Services.Interfaces;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Repositories.Interfaces.Courses.Courses;
using Studi.XUnitTests.Mocks;
using Studi.XUnitTests.Mocks.Localizers;
using Studi.XUnitTests.TestData;
using Studi.XUnitTests.Verifications;

namespace Studi.XUnitTests.CQRS.Courses.Courses;

public class GetCoursePreviewByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly CannotFindLocalizerMock _cannotFindLocalizerMock;

    private readonly GetCoursePreviewByIdHandler _sut;

    public GetCoursePreviewByIdHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _cannotFindLocalizerMock = new CannotFindLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new GetCoursePreviewByIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _mockCurrentUserService.Object,
            _cannotFindLocalizerMock);
    }

    [Fact]
    public async Task Handle_WhenCourseDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        var request = GetGetCoursePreviewByIdQuery();
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
    public async Task Handle_WhenCourseExists_ShouldReturnCoursePreview()
    {
        // Arrange
        var request = GetGetCoursePreviewByIdQuery();
        var course = CourseTestData.GetCourse();
        var coursePreviewResponseDto = CourseTestData.GetCoursePreviewResponseDto(course);

        RepositoryMocks.SetupGetSingleOrDefaultAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, course);
        MapperMocks.MockMap(_mockMapper, course, coursePreviewResponseDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        RepositoryVerifications.VerifyGetSingleOrDefaultAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapWasCalled(_mockMapper, course, coursePreviewResponseDto);
    }

    #region Helper Methods

    private static GetCoursePreviewByIdQuery GetGetCoursePreviewByIdQuery(Guid? courseId = null)
    {
        return new GetCoursePreviewByIdQuery(courseId ?? Guid.NewGuid());
    }

    #endregion
}
