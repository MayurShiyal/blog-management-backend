namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;

public class UserLoginResponse
{
    public bool Status { get; set; }

    public string Message { get; set; } = default!;

    public string? Token { get; set; }

    public object? Data { get; set; }
}
