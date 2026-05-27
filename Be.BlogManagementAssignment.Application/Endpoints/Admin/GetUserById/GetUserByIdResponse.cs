namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUserById;

public class GetUserByIdResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public GetUserByIdDto? Data { get; set; }
}
