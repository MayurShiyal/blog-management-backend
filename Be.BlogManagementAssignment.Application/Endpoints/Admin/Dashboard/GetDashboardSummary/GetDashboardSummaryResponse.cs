namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardSummary;

public class GetDashboardSummaryResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public DashboardSummaryDto? Data { get; set; }
}
