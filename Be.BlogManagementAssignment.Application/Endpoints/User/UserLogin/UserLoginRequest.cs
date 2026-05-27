using System.ComponentModel.DataAnnotations;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;

public class UserLoginRequest
{
    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
