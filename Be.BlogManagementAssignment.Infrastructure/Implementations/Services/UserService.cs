using Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Be.BlogManagementAssignment.Infrastructure.Implementations.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Services;

/// <summary>
/// Handles user registration, login, email verification, and password reset.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        JwtHelper jwtHelper,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserRegistrationResponse> RegisterAsync(
        UserRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalisedEmail = request.Email.Trim().ToLowerInvariant();

        var emailTaken = await _userRepository.ExistsAsync(normalisedEmail, cancellationToken);
        if (emailTaken)
        {
            return new UserRegistrationResponse
            {
                Status = false,
                Message = $"An account with the email '{normalisedEmail}' already exists."
            };
        }

        var rawVerificationToken = Guid.NewGuid().ToString("N");

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName?.Trim(),
            Email = normalisedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = request.Role,
            Status = UserStatus.Active,
            IsVerified = false,
            // Store the raw GUID verification token — NOT a JWT.
            // A fresh JWT is generated on every login (see LoginAsync).
            VerificationToken = rawVerificationToken
        };

        var created = await _userRepository.CreateAsync(user, cancellationToken);

        var frontendBaseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:4200";
        var verificationLink = $"{frontendBaseUrl}/auth/verify-email?token={rawVerificationToken}";

        try
        {
            await _emailService.SendVerificationEmailAsync(
                created.Email,
                created.FirstName,
                verificationLink,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send verification email to {Email}.", created.Email);
        }

        return new UserRegistrationResponse
        {
            Status = true,
            Message = $"{request.Role} registered successfully. A verification link has been sent to your email address.",
            Data = MapToData(created)
        };
    }

    /// <inheritdoc />
    public async Task<UserLoginResponse> LoginAsync(
        UserLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalisedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(normalisedEmail, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new UserLoginResponse
            {
                Status = false,
                Message = "Invalid email or password."
            };
        }

        if (!user.IsVerified)
        {
            return new UserLoginResponse
            {
                Status = false,
                Message = "Your email address has not been verified. Please check your inbox and click the verification link."
            };
        }

        // Generate a fresh JWT on every login and persist it to the database.
        var jwtToken = _jwtHelper.GenerateToken(user);

        // Store the generated JWT in the VerificationToken column so the
        // database always reflects the most recently issued token.
        user.VerificationToken = jwtToken;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation(
            "User {UserId} ({Email}, Role={Role}) logged in successfully.",
            user.Id, user.Email, user.Role);

        return new UserLoginResponse
        {
            Status = true,
            Message = "Login successful.",
            Token = jwtToken,
            Data = MapToData(user)
        };
    }

    /// <inheritdoc />
    public async Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByVerificationTokenAsync(token, cancellationToken);

        if (user is null || user.IsVerified)
        {
            return false;
        }

        user.IsVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        // Clear the one-time verification token after the email is confirmed.
        // A fresh JWT will be stored in VerificationToken on each login (see LoginAsync).
        user.VerificationToken = null;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Email verified for user {UserId} ({Email}).", user.Id, user.Email);

        return true;
    }

    /// <inheritdoc />
    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalisedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalisedEmail, cancellationToken);

        // Always return a success-like message to avoid email enumeration attacks.
        const string genericMessage =
            "If an account with that email exists, a password-reset link has been sent.";

        if (user is null)
        {
            return new ForgotPasswordResponse { Status = true, Message = genericMessage };
        }

        // Generate a secure reset token valid for 1 hour.
        var resetToken = Guid.NewGuid().ToString("N");
        var resetExpiry = DateTime.UtcNow.AddHours(1);

        user.ResetPasswordToken = resetToken;
        user.ResetPasswordExpiry = resetExpiry;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);

        var frontendBaseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:4200";
        var resetLink = $"{frontendBaseUrl}/auth/reset-password?token={resetToken}";

        try
        {
            await _emailService.SendResetPasswordEmailAsync(
                user.Email,
                user.FirstName,
                resetLink,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send reset-password email to {Email}.", user.Email);
        }

        return new ForgotPasswordResponse { Status = true, Message = genericMessage };
    }

    /// <inheritdoc />
    public async Task<ResetPasswordResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return new ResetPasswordResponse
            {
                Status = false,
                Message = "Passwords do not match."
            };
        }

        var user = await _userRepository.GetByResetPasswordTokenAsync(request.Token, cancellationToken);

        if (user is null)
        {
            return new ResetPasswordResponse
            {
                Status = false,
                Message = "Invalid or expired reset token. Please request a new password-reset link."
            };
        }

        if (user.ResetPasswordExpiry is null || user.ResetPasswordExpiry < DateTime.UtcNow)
        {
            return new ResetPasswordResponse
            {
                Status = false,
                Message = "The reset link has expired. Please request a new password-reset link."
            };
        }

        // Update password and clear reset token fields.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.ResetPasswordToken = null;
        user.ResetPasswordExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Password reset for user {UserId} ({Email}).", user.Id, user.Email);

        return new ResetPasswordResponse
        {
            Status = true,
            Message = "Your password has been reset successfully. You can now log in with your new password."
        };
    }

    private static object MapToData(User user) => new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        Role = user.Role.ToString(),
        Status = user.Status.ToString(),
        user.IsVerified,
        user.CreatedAt
    };
}