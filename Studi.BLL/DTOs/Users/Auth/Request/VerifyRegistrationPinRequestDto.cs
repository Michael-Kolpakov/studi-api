using System.ComponentModel.DataAnnotations;
using Studi.BLL.Utils.Attributes;

namespace Studi.BLL.DTOs.Users.Auth.Request;

public class VerifyRegistrationPinRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid VerificationId { get; set; }

    [Required(ErrorMessage = "Required")]
    [PinCode(ErrorMessage = "PinCodeInvalid")]
    public string PinCode { get; set; } = null!;
}
