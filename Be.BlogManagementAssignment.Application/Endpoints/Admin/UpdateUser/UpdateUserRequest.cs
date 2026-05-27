namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;
public class UpdateUserRequest
{
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
}
