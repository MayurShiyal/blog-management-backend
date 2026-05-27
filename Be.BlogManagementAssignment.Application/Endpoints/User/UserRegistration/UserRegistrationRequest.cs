using System.ComponentModel.DataAnnotations;
using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;

public class UserRegistrationRequest
{
    [Required]
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public UserRole Role { get; set; }
}
