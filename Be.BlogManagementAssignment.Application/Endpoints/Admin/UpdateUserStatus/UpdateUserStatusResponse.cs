namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;

public class UpdateUserStatusResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateUserStatusDto? Data { get; set; }
}
