using Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;

namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface IUserService
{
    /// <summary>Registers a new Author or Visitor based on the Role field in the request.</summary>
    Task<UserRegistrationResponse> RegisterAsync(
        UserRegistrationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Authenticates any user (Admin/Author/Visitor) and issues a JWT.</summary>
    Task<UserLoginResponse> LoginAsync(
        UserLoginRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Validates the verification token and marks the user's email as verified (IsVerified = true).</summary>
    Task<bool> VerifyEmailAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>Generates a reset-password token, stores it, and emails the reset link.</summary>
    Task<ForgotPasswordResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Validates the reset token, checks expiry, and updates the user's password.</summary>
    Task<ResetPasswordResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);
}
