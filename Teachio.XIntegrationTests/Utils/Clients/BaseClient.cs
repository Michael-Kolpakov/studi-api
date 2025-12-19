using RestSharp;
using RestSharp.Interceptors;

namespace Teachio.XIntegrationTests.Utils.Clients;

public class BaseClient
{
    protected string SecondPartUrl { get; }

    private readonly RestClient _client;

    public BaseClient(HttpClient client, string secondPartUrl = "")
    {
        _client = new RestClient(client)
        {
            AcceptedContentTypes = ContentType.JsonAccept
        };
        SecondPartUrl = secondPartUrl;
    }

    protected async Task<RestResponse> SendQuery(string requestString, string authToken = "")
    {
        var request = new RestRequest($"{SecondPartUrl}{requestString}");
        RestResponse response;

        try
        {
            response = await SendRequest(request, authToken);
        }
        catch (Exception ex)
        {
            return new RestResponse()
            {
                IsSuccessStatusCode = false,
                ErrorMessage = ex.Message
            };
        }

        return response;
    }

    protected async Task<RestResponse> SendCommand(
        string requestString,
        Method method,
        string authToken = "")
    {
        return await SendCommand<object>(requestString, method, null, authToken);
    }

    protected async Task<RestResponse> SendCommand<TRequestDto>(
        string requestString,
        Method method,
        TRequestDto? requestDto = null,
        string authToken = "")
        where TRequestDto : class
    {
        var request = new RestRequest($"{SecondPartUrl}{requestString}", method);

        if (requestDto is not null)
        {
            request.AddJsonBody(requestDto);
        }

        RestResponse response;

        try
        {
            response = await SendRequest(request, authToken);
        }
        catch (Exception ex)
        {
            return new RestResponse()
            {
                IsSuccessStatusCode = false,
                ErrorMessage = ex.Message
            };
        }

        return response;
    }

    private async Task<RestResponse> SendRequest(RestRequest request, string authToken)
    {
        request.Interceptors = [new ContentTypeInterceptor()];

        request.AddHeader("Authorization", $"Bearer {authToken}");
        request.AddHeader("Content-Type", "application/json");

        var response = await _client.ExecuteAsync(request);

        return response;
    }
}

public class ContentTypeInterceptor : Interceptor
{
    public override ValueTask BeforeDeserialization(RestResponse response, CancellationToken cancellationToken)
    {
        response.ContentType = "application/json";

        return ValueTask.CompletedTask;
    }
}
