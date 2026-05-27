namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Status { get; set; } = default!;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
