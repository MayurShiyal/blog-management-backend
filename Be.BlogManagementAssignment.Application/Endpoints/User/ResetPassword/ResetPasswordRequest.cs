using System.ComponentModel.DataAnnotations;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Reset token is required.")]
    public string Token { get; set; } = default!;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "Confirm password is required.")]
    public string ConfirmPassword { get; set; } = default!;
}
