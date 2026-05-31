using AutoMapper;
using Moq;
using Studi.BLL.CQRS.Courses.Courses.GetPaginated;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.BLL.Services.Interfaces;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Repositories.Interfaces.Courses.Courses;
using Studi.DAL.Utils.Helpers;
using Studi.XUnitTests.Mocks;
using Studi.XUnitTests.Mocks.Localizers;
using Studi.XUnitTests.TestData;
using Studi.XUnitTests.Verifications;
using CourseEntity = Studi.DAL.Entities.Courses.Courses.Course;

namespace Studi.XUnitTests.CQRS.Courses.Courses;

public class GetPaginatedCoursesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly BllLocalizerMock _bllLocalizerMock;

    private readonly GetPaginatedCoursesHandler _sut;

    public GetPaginatedCoursesHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _bllLocalizerMock = new BllLocalizerMock();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new GetPaginatedCoursesHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object,
            _mockCurrentUserService.Object,
            _bllLocalizerMock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task Handle_ShouldReturnPaginatedCoursesWithCorrectTotalAmount(int coursesCount)
    {
        // Arrange
        const ushort pageNumber = 1;
        const ushort pageSize = 10;
        var request = GetGetPaginatedCoursesQuery(pageNumber, pageSize);

        var courses = Enumerable.Range(1, coursesCount)
            .Select(i => CourseTestData.GetCourse(courseNumber: i))
            .ToList();

        var paginatedCourses = PaginationResponse<CourseEntity>.Create(courses, coursesCount, pageNumber, pageSize);
        var mappedCourses = courses.Select((c, index) => CourseTestData.GetCoursePreviewShortResponseDto(c, index)).ToList();

        RepositoryMocks.SetupGetAllPaginatedAsyncMock(_mockRepository, wrapper => wrapper.CoursesRepository, paginatedCourses);
        MapperMocks.MockMapCollection<CourseEntity, CoursePreviewShortResponseDto>(_mockMapper, mappedCourses);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(coursesCount, result.Value.TotalAmount);
        Assert.Equal(mappedCourses.Count, result.Value.Courses.Count());
        RepositoryVerifications.VerifyGetAllPaginatedAsyncWasCalled(_mockRepository, wrapper => wrapper.CoursesRepository);
        MapperVerifications.VerifyMapCollectionWasCalled<CourseEntity, CoursePreviewShortResponseDto>(_mockMapper);
    }

    #region Helper Methods

    private static GetPaginatedCoursesQuery GetGetPaginatedCoursesQuery(
        ushort pageNumber = 1,
        ushort pageSize = 10)
    {
        return new GetPaginatedCoursesQuery(pageNumber, pageSize);
    }

    #endregion
}
