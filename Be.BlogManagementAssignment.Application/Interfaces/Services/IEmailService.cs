namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface IEmailService
{
    /// <summary>Sends an email verification link to the specified recipient.</summary>
    Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationLink,
        CancellationToken cancellationToken = default);

    /// <summary>Sends a password-reset link to the specified recipient.</summary>
    Task SendResetPasswordEmailAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken cancellationToken = default);
}
