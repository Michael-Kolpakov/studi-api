using MimeKit;

namespace Teachio.BLL.Models.Email.Base;

/// <summary>
/// Represents the <see cref="MessageData"/> type.
/// </summary>
public abstract class MessageData
{
    public IEnumerable<string> To { get; set; } = new List<string>();

    /// <summary>
    /// Performs the ToMimeMessage operation.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    public abstract MimeMessage ToMimeMessage();
}
