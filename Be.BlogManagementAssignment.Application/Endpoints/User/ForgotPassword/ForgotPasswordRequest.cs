using System.ComponentModel.DataAnnotations;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;

public class ForgotPasswordRequest
{
    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;
}
