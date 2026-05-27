using System.ComponentModel.DataAnnotations;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public string NewPassword { get; set; } = default!;

    [Required]
    public string ConfirmPassword { get; set; } = default!;
}
