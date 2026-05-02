using RestSharp;
using Teachio.BLL.DTOs.Courses.Courses.Request.Create;
using Teachio.BLL.DTOs.Courses.Courses.Request.Update;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.XIntegrationTests.Utils.Clients;

public class CourseClient(HttpClient client, string secondPathUrl = "")
    : BaseClient(client, secondPathUrl)
{
    public async Task<RestResponse> GetPaginatedAsync(int? pageNumber, int? pageSize, string authToken = "")
    {
        var paginationParams = $"pageNumber={pageNumber}&pageSize={pageSize}";

        return await SendQuery($"/get-paginated?{paginationParams}", authToken);
    }

    public async Task<RestResponse> GetByIdAsync(Guid id, string authToken = "")
    {
        return await SendQuery($"/get-by-id/{id}", authToken);
    }

    public async Task<RestResponse> GetByIdPreviewAsync(Guid id, string authToken = "")
    {
        return await SendQuery($"/get-by-id-preview/{id}", authToken);
    }

    public async Task<RestResponse> CreateAsync(CourseCreateRequestDto courseCreateRequestDto, string authToken = "")
    {
        return await SendCommand("/create", Method.Post, courseCreateRequestDto, authToken);
    }

    public async Task<RestResponse> UpdateAsync(CourseUpdateRequestDto courseUpdateRequestDto, string authToken = "")
    {
        return await SendCommand("/update", Method.Put, courseUpdateRequestDto, authToken);
    }

    public async Task<RestResponse> DeleteAsync(Guid id, string authToken = "")
    {
        return await SendCommand($"/delete/{id}", Method.Delete, new CourseResponseDto(), authToken);
    }
}
