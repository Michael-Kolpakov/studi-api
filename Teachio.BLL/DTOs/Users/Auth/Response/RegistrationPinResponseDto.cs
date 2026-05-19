namespace Teachio.BLL.DTOs.Users.Auth.Response;

public class RegistrationPinResponseDto
{
    public Guid VerificationId { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime ResendAvailableAtUtc { get; set; }
}
