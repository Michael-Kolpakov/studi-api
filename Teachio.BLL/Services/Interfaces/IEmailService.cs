using Teachio.BLL.Models.Email.Base;

namespace Teachio.BLL.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(MessageData messageData);
}
