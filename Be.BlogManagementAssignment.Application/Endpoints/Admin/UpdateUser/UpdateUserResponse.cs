namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;

public class UpdateUserResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateUserDto? Data { get; set; }
}
