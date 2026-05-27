using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Users.Auth.Request;

public class VerifyRegistrationPinRequestDto
{
    public Guid? VerificationId { get; set; }

    [Required(ErrorMessage = "Required")]
    public string PinCode { get; set; } = null!;
}
