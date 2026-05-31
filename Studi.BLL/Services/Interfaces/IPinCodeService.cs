namespace Studi.BLL.Services.Interfaces;

public interface IPinCodeService
{
    string GeneratePin();

    string HashPin(string pin);

    bool VerifyPin(string pin, string? storedHash);

    bool IsValidPin(string? pin);
}
