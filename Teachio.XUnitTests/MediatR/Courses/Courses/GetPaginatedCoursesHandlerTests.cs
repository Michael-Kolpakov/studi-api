using AutoMapper;
using Moq;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.MediatR.Courses.Courses.GetPaginated;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Utils.Helpers;
using Teachio.XUnitTests.Mocks;
using Teachio.XUnitTests.TestData;
using Teachio.XUnitTests.Verifications;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.XUnitTests.MediatR.Courses.Courses;

public class GetPaginatedCoursesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;

    private readonly GetPaginatedCoursesHandler _sut;

    public GetPaginatedCoursesHandlerTests()
    {
        var mockCoursesRepository = new Mock<ICoursesRepository>();

        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();

        _mockRepository
            .Setup(x => x.CoursesRepository)
            .Returns(mockCoursesRepository.Object);

        _sut = new GetPaginatedCoursesHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLoggerService.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task Handle_ShouldReturnPaginatedCoursesWithCorrectTotalAmount(int coursesCount)
    {
        // Arrange
        const ushort PageNumber = 1;
        const ushort PageSize = 10;
        var request = GetGetPaginatedCoursesQuery(PageNumber, PageSize);

        var courses = Enumerable.Range(1, coursesCount)
            .Select(i => CourseTestData.GetCourse(courseNumber: i))
            .ToList();

        var paginatedCourses = PaginationResponse<CourseEntity>.Create(courses, coursesCount, PageNumber, PageSize);
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
        ushort pageSize = 10,
        Guid? requestingUserId = null)
    {
        return new GetPaginatedCoursesQuery(pageNumber, pageSize, requestingUserId ?? Guid.NewGuid());
    }

    #endregion
}
