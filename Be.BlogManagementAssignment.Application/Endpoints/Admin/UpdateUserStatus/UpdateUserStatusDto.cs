namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;

public class UpdateUserStatusDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
}
