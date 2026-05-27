using Google.Apis.Auth;

namespace Studi.BLL.Services.Interfaces;

public interface IGoogleService
{
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string id);
}
