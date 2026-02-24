using System.Net;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.WebApi;
using Teachio.XIntegrationTests.Utils;
using Teachio.XIntegrationTests.Utils.BeforeAndAfterAttributes.Courses.Courses;
using Teachio.XIntegrationTests.Utils.Clients;
using Teachio.XIntegrationTests.Utils.Extractors;

namespace Teachio.XIntegrationTests.ControllerTests;

public class CourseControllerTests : BaseControllerTests<CourseClient>
{
    private readonly Course _testCourse;
    
    public CourseControllerTests(CustomWebApplicationFactory<Program> factory)
        : base(factory, "/api/courses")
    {
        var courseId = Guid.NewGuid();
        var appUserId = Guid.NewGuid();

        _testCourse = CourseExtractor.Extract(courseId, appUserId);
    }

    #region GetPaginated Tests

    [Fact]
    public async Task GetPaginated_WhenPaginationIsValid_ShouldReturnPaginatedCourses()
    {
        // Arrange
        const int pageNumber = 1;
        const int pageSize = 10;
        
        // Act
        var response = await Client.GetPaginatedAsync(pageNumber, pageSize);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<PaginatedCoursesResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returnedValue);
        Assert.True(returnedValue.TotalAmount >= 1);
        Assert.Contains(returnedValue.Courses, course => course.Id == _testCourse.Id);
    }

    [Theory]
    [InlineData(-2, -2)]
    [InlineData(null, null)]
    public async Task GetPaginated_WhenPaginationIsInvalid_ShouldReturnBadRequest(int? pageNumber, int? pageSize)
    {
        // Arrange

        // Act
        var response = await Client.GetPaginatedAsync(pageNumber, pageSize);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenCourseIdIsValid_ShouldReturnCourse()
    {
        // Arrange
        var courseId = _testCourse.Id;

        // Act
        var response = await Client.GetByIdAsync(courseId);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CourseResponseDto>(response.Content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returnedValue);

        Assert.Multiple(
            () => Assert.Equal(_testCourse.Id, returnedValue.Id),
            () => Assert.Equal(_testCourse.Title, returnedValue.Title),
            () => Assert.Equal(_testCourse.Description, returnedValue.Description),
            () => Assert.Equal(_testCourse.SectionsCount, returnedValue.SectionsCount),
            () => Assert.Equal(_testCourse.WatchingUsersCount, returnedValue.WatchingUsersCount),
            () => Assert.Equal(_testCourse.OwnerUserId, returnedValue.OwnerUserId));
    }

    [Fact]
    public async Task GetById_WhenCourseIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var response = await Client.GetByIdAsync(courseId);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CourseResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(returnedValue);
    }

    #endregion

    #region GetByIdPreview Tests

    [Fact]
    public async Task GetByIdPreview_WhenCourseIdIsValid_ShouldReturnCourse()
    {
        // Arrange
        var courseId = _testCourse.Id;

        // Act
        var response = await Client.GetByIdPreviewAsync(courseId);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CoursePreviewResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returnedValue);

        Assert.Multiple(
            () => Assert.Equal(_testCourse.Id, returnedValue.Id),
            () => Assert.Equal(_testCourse.Title, returnedValue.Title),
            () => Assert.Equal(_testCourse.Description, returnedValue.Description),
            () => Assert.Equal(_testCourse.SectionsCount, returnedValue.SectionsCount),
            () => Assert.Equal(_testCourse.WatchingUsersCount, returnedValue.WatchingUsersCount),
            () => Assert.Equal(_testCourse.OwnerUserId, returnedValue.OwnerUserId));
    }

    [Fact]
    public async Task GetByIdPreview_WhenCourseIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var response = await Client.GetByIdPreviewAsync(courseId);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CoursePreviewResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(returnedValue);
    }

    #endregion

    #region Create Tests

    [Fact]
    [ExtractCreateTestCourse]
    public async Task Create_WhenCourseCreateRequestDtoIsValid_ShouldCreateCourse()
    {
        // Arrange
        var courseCreateRequestDto = ExtractCreateTestCourseAttribute.CourseCreateRequestDto;
        
        // Act
        var response = await Client.CreateAsync(courseCreateRequestDto);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CourseResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returnedValue);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, returnedValue.Id),
            () => Assert.Equal(courseCreateRequestDto.Title, returnedValue.Title),
            () => Assert.Equal(courseCreateRequestDto.Description, returnedValue.Description));
    }

    [Fact]
    [ExtractCreateTestCourse]
    public async Task Create_WhenCourseCreateRequestDtoIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var courseCreateRequestDto = ExtractCreateTestCourseAttribute.CourseCreateRequestDto;
        courseCreateRequestDto.Title = null!;

        // Act
        var response = await Client.CreateAsync(courseCreateRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [ExtractCreateTestCourse]
    public async Task Create_WhenCourseForUserWithSuchNameAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        var courseCreateRequestDto = ExtractCreateTestCourseAttribute.CourseCreateRequestDto;
        courseCreateRequestDto.Title = _testCourse.Title;

        // Act
        var response = await Client.CreateAsync(courseCreateRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Update Tests

    [Fact]
    [ExtractUpdateTestCourse]
    public async Task Update_WhenCourseUpdateRequestDtoIsValid_ShouldUpdateCourse()
    {
        // Arrange
        var courseUpdateRequestDto = ExtractUpdateTestCourseAttribute.CourseUpdateRequestDto;
        
        // Act
        var response = await Client.UpdateAsync(courseUpdateRequestDto);
        var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CourseResponseDto>(response.Content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returnedValue);

        Assert.Multiple(
            () => Assert.Equal(courseUpdateRequestDto.Id, returnedValue.Id),
            () => Assert.Equal(courseUpdateRequestDto.Title, returnedValue.Title),
            () => Assert.Equal(courseUpdateRequestDto.Description, returnedValue.Description));
    }

    [Fact]
    [ExtractUpdateTestCourse]
    public async Task Update_WhenCourseUpdateRequestDtoIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var courseUpdateRequestDto = ExtractUpdateTestCourseAttribute.CourseUpdateRequestDto;
        courseUpdateRequestDto.Title = null!;

        // Act
        var response = await Client.UpdateAsync(courseUpdateRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [ExtractUpdateTestCourse]
    public async Task Update_WhenCourseDoesNotExists_ShouldReturnBadRequest()
    {
        // Arrange
        var courseUpdateRequestDto = ExtractUpdateTestCourseAttribute.CourseUpdateRequestDto;
        courseUpdateRequestDto.Id = Guid.NewGuid();

        // Act
        var response = await Client.UpdateAsync(courseUpdateRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Delete Tests

    [Fact]
    [ExtractDeleteTestCourse]
    public async Task Delete_WhenCourseIdIsValid_ShouldDeleteCourse()
    {
        // Arrange
        var expectedCourse = ExtractDeleteTestCourseAttribute.Course;
        var courseId = expectedCourse.Id;

         // Act
         var response = await Client.DeleteAsync(courseId);
         var returnedValue = CaseInsensitiveJsonDeserializer.Deserialize<CourseResponseDto>(response.Content);

         // Assert
         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
         Assert.NotNull(returnedValue);

         Assert.Multiple(
            () => Assert.Equal(expectedCourse.Id, returnedValue.Id),
            () => Assert.Equal(expectedCourse.Title, returnedValue.Title),
            () => Assert.Equal(expectedCourse.Description, returnedValue.Description),
            () => Assert.Equal(expectedCourse.SectionsCount, returnedValue.SectionsCount),
            () => Assert.Equal(expectedCourse.WatchingUsersCount, returnedValue.WatchingUsersCount),
            () => Assert.Equal(expectedCourse.OwnerUserId, returnedValue.OwnerUserId));
    }

    [Fact]
    [ExtractDeleteTestCourse]
    public async Task Delete_WhenCourseIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync(courseId);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CourseExtractor.Remove(_testCourse);
        }

        base.Dispose(disposing);
    }
}
