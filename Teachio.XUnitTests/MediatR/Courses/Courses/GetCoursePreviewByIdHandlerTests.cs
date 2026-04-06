using AutoMapper;
using Moq;
using Teachio.BLL.MediatR.Courses.Courses.GetByIdPreview;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.Mocks.Localizers;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

public class GetCoursePreviewByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly CannotFindLocalizerMock _cannotFindLocalizerMock;

    private readonly GetCoursePreviewByIdHandler _sut;

    public GetCoursePreviewByIdHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _cannotFindLocalizerMock = new CannotFindLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new GetCoursePreviewByIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
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
