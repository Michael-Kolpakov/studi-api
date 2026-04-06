using Google.Apis.Auth;

namespace Teachio.BLL.Services.Interfaces;

public interface IGoogleService
{
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string id);
}
