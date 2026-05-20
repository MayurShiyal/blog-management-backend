using System.Net;
using System.Net.Mail;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Utilities;

/// <summary>
/// Sends transactional emails via SMTP using System.Net.Mail (no third-party dependency).
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private (string Host, int Port, string Email, string Password, bool EnableSsl) GetSmtpOptions()
    {
        var smtpSection = _configuration.GetSection("Smtp");

        var host     = smtpSection["Host"]     ?? throw new InvalidOperationException("SMTP Host is not configured.");
        var email    = smtpSection["Email"]    ?? throw new InvalidOperationException("SMTP Email is not configured.");
        var password = smtpSection["Password"] ?? throw new InvalidOperationException("SMTP Password is not configured.");
        var port     = int.TryParse(smtpSection["Port"], out var p) ? p : 587;
        var enableSsl = !string.Equals(smtpSection["EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);

        return (host, port, email, password, enableSsl);
    }

    private SmtpClient CreateClient()
    {
        var (host, port, email, password, enableSsl) = GetSmtpOptions();

        return new SmtpClient(host, port)
        {
            Credentials  = new NetworkCredential(email, password),
            EnableSsl    = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    /// <inheritdoc />
    public async Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationLink,
        CancellationToken cancellationToken = default)
    {
        var (_, _, fromEmail, _, _) = GetSmtpOptions();

        using var client  = CreateClient();
        using var message = new MailMessage
        {
            From       = new MailAddress(fromEmail, "Blog Management System"),
            Subject    = "Verify Your Email Address — Blog Management System",
            Body       = BuildVerificationHtml(toName, verificationLink),
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(toEmail, toName));

        await client.SendMailAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SendResetPasswordEmailAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var (_, _, fromEmail, _, _) = GetSmtpOptions();

        using var client  = CreateClient();
        using var message = new MailMessage
        {
            From       = new MailAddress(fromEmail, "Blog Management System"),
            Subject    = "Reset Your Password — Blog Management System",
            Body       = BuildResetPasswordHtml(toName, resetLink),
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(toEmail, toName));

        await client.SendMailAsync(message, cancellationToken);
    }

    private static string BuildVerificationHtml(string toName, string verificationLink) => $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8' />
  <title>Email Verification</title>
  <style>
    body {{ margin: 0; padding: 0; background-color: #f3f4f6; font-family: Arial, sans-serif; }}
    .wrapper {{ width: 100%; background-color: #f3f4f6; padding: 20px 0; }}
    .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }}
    .header {{ background-color: #3730A3; padding: 20px 30px; text-align: center; }}
    .header h2 {{ color: #ffffff; margin: 0; font-size: 20px; }}
    .body {{ padding: 30px 24px; }}
    .footer {{ background-color: #f9fafb; padding: 12px; text-align: center; border-top: 1px solid #e5e7eb; }}
    .footer p {{ font-size: 12px; color: #9ca3af; margin: 0; }}
    p {{ color: #374151; line-height: 1.6; margin: 0 0 12px 0; font-size: 14px; }}
    .btn-wrap {{ text-align: center; margin: 28px 0; }}
    .btn {{ background-color: #3730A3; color: #ffffff !important; padding: 12px 28px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block; }}
    .link {{ word-break: break-all; font-size: 13px; color: #3730A3; }}
    .note {{ color: #6b7280; font-size: 13px; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='container'>
      <div class='header'><h2>Blog Management System</h2></div>
      <div class='body'>
        <p>Hello <strong>{toName}</strong>,</p>
        <p>Thank you for registering. Please verify your email address by clicking the button below:</p>
        <div class='btn-wrap'><a href='{verificationLink}' class='btn'>Verify Email Address</a></div>
        <p>Or copy and paste this link into your browser:</p>
        <p class='link'><a href='{verificationLink}'>{verificationLink}</a></p>
        <p class='note'>If you did not create an account, please ignore this email.</p>
        <p style='margin-top:24px;'>Regards,<br><strong>Blog Management System Team</strong></p>
      </div>
      <div class='footer'><p>&copy; {DateTime.UtcNow.Year} Blog Management System. All rights reserved.</p></div>
    </div>
  </div>
</body>
</html>";

    private static string BuildResetPasswordHtml(string toName, string resetLink) => $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8' />
  <title>Reset Your Password</title>
  <style>
    body {{ margin: 0; padding: 0; background-color: #f3f4f6; font-family: Arial, sans-serif; }}
    .wrapper {{ width: 100%; background-color: #f3f4f6; padding: 20px 0; }}
    .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }}
    .header {{ background-color: #DC2626; padding: 20px 30px; text-align: center; }}
    .header h2 {{ color: #ffffff; margin: 0; font-size: 20px; }}
    .body {{ padding: 30px 24px; }}
    .footer {{ background-color: #f9fafb; padding: 12px; text-align: center; border-top: 1px solid #e5e7eb; }}
    .footer p {{ font-size: 12px; color: #9ca3af; margin: 0; }}
    p {{ color: #374151; line-height: 1.6; margin: 0 0 12px 0; font-size: 14px; }}
    .btn-wrap {{ text-align: center; margin: 28px 0; }}
    .btn {{ background-color: #DC2626; color: #ffffff !important; padding: 12px 28px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block; }}
    .link {{ word-break: break-all; font-size: 13px; color: #DC2626; }}
    .note {{ color: #6b7280; font-size: 13px; }}
    .warning-box {{ background-color: #FEF2F2; border: 1px solid #FECACA; border-radius: 6px; padding: 12px 16px; margin-bottom: 16px; }}
    .warning-box p {{ color: #991B1B; margin: 0; font-size: 13px; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='container'>
      <div class='header'><h2>Password Reset Request</h2></div>
      <div class='body'>
        <p>Hello <strong>{toName}</strong>,</p>
        <p>We received a request to reset the password for your Blog Management System account.</p>
        <p>Click the button below to reset your password. This link is valid for <strong>1 hour</strong>.</p>
        <div class='btn-wrap'><a href='{resetLink}' class='btn'>Reset My Password</a></div>
        <p>Or copy and paste this link into your browser:</p>
        <p class='link'><a href='{resetLink}'>{resetLink}</a></p>
        <div class='warning-box'>
          <p>&#9888; If you did not request a password reset, please ignore this email. Your password will not be changed.</p>
        </div>
        <p style='margin-top:24px;'>Regards,<br><strong>Blog Management System Team</strong></p>
      </div>
      <div class='footer'><p>&copy; {DateTime.UtcNow.Year} Blog Management System. All rights reserved.</p></div>
    </div>
  </div>
</body>
</html>";
}
