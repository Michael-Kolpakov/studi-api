using System.Net;
using MimeKit;
using Studi.BLL.Models.Email.Base;

namespace Studi.BLL.Models.Email;

public class RegistrationPinEmailMessageData : MessageData
{
    public string FullName { get; set; } = null!;

    public string PinCode { get; set; } = null!;

    public string Subject { get; set; } = "Registration on the Studi website";

    public override MimeMessage ToMimeMessage()
    {
        var message = new MimeMessage();

        foreach (var recipient in To)
        {
            message.To.Add(MailboxAddress.Parse(recipient));
        }

        message.Subject = Subject;

        var safeName = WebUtility.HtmlEncode(FullName);
        var safePin = WebUtility.HtmlEncode(PinCode);

        var htmlBody = $@"<!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='utf-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                <title>Studi Verification</title>
            </head>
            <body style='margin:0;padding:0;background-color:#eef1f4;'>
                <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#eef1f4;'>
                    <tr>
                        <td align='center' style='padding:40px 16px;'>
                            <table role='presentation' width='600' cellpadding='0' cellspacing='0' style='width:600px;max-width:100%;background-color:#ffffff;border:1px solid #d9dee4;border-radius:12px;'>
                                <tr>
                                    <td style='padding:32px 36px;text-align:left;font-family:Arial, Helvetica, sans-serif;color:#1a1a1a;'>
                                        <div style='font-size:22px;font-weight:700;text-align:center;'>Studi Learning Platform</div>
                                        <div style='height:1px;background-color:#4a4a4a;margin:16px 0;'></div>
                                        <div style='font-size:16px;font-weight:600;'>Hi {safeName},</div>
                                        <div style='height:16px;'></div>
                                        <p style='margin:0;font-size:15px;line-height:1.6;'>
                                            We received a request from your email to create an account on Studi. Please enter the code below to verify your ownership of this email. If it wasn't you, then just ignore this message. Do not share this code with anyone.
                                        </p>
                                        <div style='height:16px;'></div>
                                        <p style='margin:0;font-size:15px;line-height:1.6;'>Enjoy using our platform!</p>
                                        <p style='margin:0;font-size:15px;line-height:1.6;'>Sincerely, Studi Corporation®</p>
                                        <div style='height:20px;'></div>
                                        <div style='background-color:#e6f4ea;border:1px solid #b7dfc4;border-radius:0;padding:16px;text-align:center;'>
                                            <div style='font-size:14px;font-weight:700;color:#1a7f37;'>Your verification code:</div>
                                            <div style='font-size:22px;font-weight:800;color:#1a7f37;letter-spacing:2px;'>{safePin}</div>
                                        </div>
                                        <div style='height:20px;'></div>
                                        <div style='text-align:center;font-size:12px;color:#6b6b6b;'>Studi Corporation®</div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

        var textBody = $"Hi {FullName},\n\n" +
            "We received a request from your email to create an account on Studi. Please enter the code below to verify your ownership of this email. If it wasn't you, then just ignore this message. Do not share this code with anyone.\n\n" +
            "Enjoy using our platform!\n" +
            "Sincerely, Studi Corporation®\n\n" +
            $"Your verification code: {PinCode}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = textBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }
}
