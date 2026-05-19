using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Teachio.BLL.Models.Auth;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.BLL.Services.Realizations;

public class PinCodeService : IPinCodeService
{
    private readonly EmailVerificationOptions _options;

    public PinCodeService(IOptions<EmailVerificationOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        ValidateOptions(_options);
    }

    public string GeneratePin()
    {
        var length = _options.PinLength;
        if (length <= 0)
        {
            throw new InvalidOperationException("Email verification PIN length must be positive.");
        }

        var digits = new char[length];

        for (var i = 0; i < length; i++)
        {
            var digit = RandomNumberGenerator.GetInt32(0, 10);
            digits[i] = (char)('0' + digit);
        }

        return new string(digits);
    }

    public string HashPin(string pin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pin);

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPin(string pin, string? storedHash)
    {
        if (string.IsNullOrWhiteSpace(pin) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        try
        {
            var computedHash = HashPin(pin);

            return FixedTimeEquals(computedHash, storedHash);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public bool IsValidPin(string? pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
        {
            return false;
        }

        if (pin.Length != _options.PinLength)
        {
            return false;
        }

        return pin.All(char.IsDigit);
    }

    private static bool FixedTimeEquals(string leftHash, string rightHash)
    {
        try
        {
            var leftBytes = Convert.FromBase64String(leftHash);
            var rightBytes = Convert.FromBase64String(rightHash);

            return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static void ValidateOptions(EmailVerificationOptions options)
    {
        if (options.PinLength <= 0)
        {
            throw new InvalidOperationException("Email verification PIN length must be positive.");
        }

        if (options.PinExpiresInMinutes <= 0)
        {
            throw new InvalidOperationException("Email verification PIN expiry must be positive.");
        }

        if (options.ResendCooldownMinutes <= 0)
        {
            throw new InvalidOperationException("Email verification resend cooldown must be positive.");
        }
    }
}
