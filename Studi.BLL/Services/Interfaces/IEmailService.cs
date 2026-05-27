using Studi.BLL.Models.Email.Base;

namespace Studi.BLL.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(MessageData messageData);
}
