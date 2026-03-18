using RestSharp;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.XIntegrationTests.Utils.Clients;

/// <summary>
/// Represents the <see cref="CourseClient"/> type.
/// </summary>
public class CourseClient(HttpClient client, string secondPathUrl = "")
    : BaseClient(client, secondPathUrl)
{
    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> GetPaginatedAsync(int? pageNumber, int? pageSize, string authToken = "")
    {
        var paginationParams = $"pageNumber={pageNumber}&pageSize={pageSize}";

        return await SendQuery($"/get-paginated?{paginationParams}", authToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="id">The identifier of <paramref name="id"/>.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> GetByIdAsync(Guid id, string authToken = "")
    {
        return await SendQuery($"/get-by-id/{id}", authToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="id">The identifier of <paramref name="id"/>.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> GetByIdPreviewAsync(Guid id, string authToken = "")
    {
        return await SendQuery($"/get-by-id-preview/{id}", authToken);
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="courseCreateRequestDto">The request payload in <paramref name="courseCreateRequestDto"/>.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> CreateAsync(CourseCreateRequestDto courseCreateRequestDto, string authToken = "")
    {
        return await SendCommand("/create", Method.Post, courseCreateRequestDto, authToken);
    }

    /// <summary>
    /// Updates the target entity.
    /// </summary>
    /// <param name="courseUpdateRequestDto">The request payload in <paramref name="courseUpdateRequestDto"/>.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> UpdateAsync(CourseUpdateRequestDto courseUpdateRequestDto, string authToken = "")
    {
        return await SendCommand("/update", Method.Put, courseUpdateRequestDto, authToken);
    }

    /// <summary>
    /// Deletes the target entity.
    /// </summary>
    /// <param name="id">The identifier of <paramref name="id"/>.</param>
    /// <param name="authToken">The <paramref name="authToken"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<RestResponse> DeleteAsync(Guid id, string authToken = "")
    {
        return await SendCommand($"/delete/{id}", Method.Delete, new CourseResponseDto(), authToken);
    }
}
