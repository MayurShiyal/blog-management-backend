namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
