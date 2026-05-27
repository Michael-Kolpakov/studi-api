namespace Studi.BLL.Models.Auth;

public class EmailVerificationOptions
{
    public const string SectionName = "EmailVerification";

    public int PinLength { get; set; } = 6;

    public int PinExpiresInMinutes { get; set; } = 5;

    public int ResendCooldownMinutes { get; set; } = 1;

    public string Subject { get; set; } = "Registration on the Studi website";
}
