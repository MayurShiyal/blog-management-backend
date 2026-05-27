using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = default!;

    public string? LastName { get; set; }

    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public UserRole Role { get; set; }

    public UserStatus Status { get; set; }

    public bool IsVerified { get; set; }

    public string? VerificationToken { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordExpiry { get; set; }

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
