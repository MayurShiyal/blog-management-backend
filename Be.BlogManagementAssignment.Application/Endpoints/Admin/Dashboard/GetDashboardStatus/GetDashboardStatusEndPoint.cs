using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;

public sealed class GetDashboardStatusEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/status",
            async (IDashboardService dashboardService, CancellationToken cancellationToken) =>
            {
                var result = await dashboardService.GetStatusAsync(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("getDashboardStatus")
            .WithSummary("Get dashboard active/inactive status counts (Admin only)")
            .WithDescription(
                "Returns active and inactive counts for users (non-admin, non-deleted) " +
                "and categories (non-deleted).")
            .Produces<GetDashboardStatusResponse>(200)
            .RequireAuthorization("AdminOnly");
    }
}
