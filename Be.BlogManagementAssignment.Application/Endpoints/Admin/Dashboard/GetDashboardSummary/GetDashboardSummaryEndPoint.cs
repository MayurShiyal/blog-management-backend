using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardSummary;

public sealed class GetDashboardSummaryEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/summary",
            async (IDashboardService dashboardService, CancellationToken cancellationToken) =>
            {
                var result = await dashboardService.GetSummaryAsync(cancellationToken);
                return Results.Ok(result);
            })
            .WithName("getDashboardSummary")
            .WithSummary("Get dashboard summary counts (Admin only)")
            .WithDescription(
                "Returns total counts of active (non-deleted) users (admins excluded), " +
                "blogs, and categories.")
            .Produces<GetDashboardSummaryResponse>(200)
            .RequireAuthorization("AdminOnly");
    }
}
