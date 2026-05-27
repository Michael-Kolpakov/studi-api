using RestSharp;
using RestSharp.Interceptors;

namespace Studi.XIntegrationTests.Utils.Clients;

public class BaseClient : IDisposable
{
    protected string SecondPartUrl { get; }

    private readonly RestClient _client;
    private bool _disposed;

    public BaseClient(HttpClient client, string secondPartUrl = "")
    {
        _client = new RestClient(client)
        {
            AcceptedContentTypes = ContentType.JsonAccept
        };
        SecondPartUrl = secondPartUrl;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _client.Dispose();
        }

        _disposed = true;
    }

    protected async Task<RestResponse> SendQuery(string requestString, string authToken = "")
    {
        ThrowIfDisposed();

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
        ThrowIfDisposed();

        return await SendCommand<object>(requestString, method, null, authToken);
    }

    protected async Task<RestResponse> SendCommand<TRequestDto>(
        string requestString,
        Method method,
        TRequestDto? requestDto = null,
        string authToken = "")
        where TRequestDto : class
    {
        ThrowIfDisposed();

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
        ThrowIfDisposed();

        request.Interceptors = [new ContentTypeInterceptor()];

        request.AddHeader("Authorization", $"Bearer {authToken}");
        request.AddHeader("Content-Type", "application/json");

        var response = await _client.ExecuteAsync(request);

        return response;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(BaseClient));
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
