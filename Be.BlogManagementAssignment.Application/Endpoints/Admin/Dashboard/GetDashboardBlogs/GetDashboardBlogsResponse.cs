namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;

public class GetDashboardBlogsResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public DashboardBlogsDto? Data { get; set; }
}
