namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;

public class UserRegistrationResponse
{
    public bool Status { get; set; }

    public string Message { get; set; } = default!;

    public object? Data { get; set; }
}
