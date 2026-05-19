namespace Teachio.BLL.Models.Email;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = null!;

    public int Port { get; set; } = 587;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string FromAddress { get; set; } = null!;

    public string FromName { get; set; } = "Studi";

    public string SecureSocketOptions { get; set; } = "StartTls";
}
