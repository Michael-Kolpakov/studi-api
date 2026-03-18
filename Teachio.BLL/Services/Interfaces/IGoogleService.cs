using Google.Apis.Auth;

namespace Teachio.BLL.Services.Interfaces;

/// <summary>
/// Defines the contract for <see cref="IGoogleService"/>.
/// </summary>
public interface IGoogleService
{
    /// <summary>
    /// Validates a Google ID token and returns its payload.
    /// </summary>
    /// <param name="id">The Google ID token to validate.</param>
    /// <returns>A task that represents the asynchronous operation and contains the validated token payload.</returns>
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string id);
}
