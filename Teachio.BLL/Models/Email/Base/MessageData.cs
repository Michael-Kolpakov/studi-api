using MimeKit;

namespace Teachio.BLL.Models.Email.Base;

public abstract class MessageData
{
    public IEnumerable<string> To { get; set; } = new List<string>();

    public abstract MimeMessage ToMimeMessage();
}
