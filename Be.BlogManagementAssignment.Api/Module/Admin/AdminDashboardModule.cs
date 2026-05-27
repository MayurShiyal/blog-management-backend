using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardSummary;

namespace Be.BlogManagementAssignment.Api.Module.Admin;
public class AdminDashboardModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/admin/dashboard")
            .WithTags("Admin - Dashboard");

        group.MapEndPoints<GetDashboardSummaryEndPoint>();
        group.MapEndPoints<GetDashboardStatusEndPoint>();
        group.MapEndPoints<GetDashboardBlogsEndPoint>();
    }
}
