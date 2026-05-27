namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;

public class GetDashboardStatusResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public DashboardStatusDto? Data { get; set; }
}
