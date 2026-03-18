using Teachio.BLL.Models.Email.Base;

namespace Teachio.BLL.Services.Interfaces;

/// <summary>
/// Defines the contract for <see cref="IEmailService"/>.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="messageData">The message payload to send.</param>
    /// <returns>A task that represents the asynchronous operation and contains <see langword="true"/> when the email is sent successfully; otherwise, <see langword="false"/>.</returns>
    Task<bool> SendEmailAsync(MessageData messageData);
}
