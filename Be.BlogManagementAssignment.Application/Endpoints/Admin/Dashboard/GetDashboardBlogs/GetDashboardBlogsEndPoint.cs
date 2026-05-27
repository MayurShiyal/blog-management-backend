using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;

public sealed class GetDashboardBlogsEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/blogs",
            async (
                IDashboardService dashboardService,
                int latestCount = 5,
                int monthsBack = 12,
                CancellationToken cancellationToken = default) =>
            {
                var result = await dashboardService.GetBlogsAsync(latestCount, monthsBack, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("getDashboardBlogs")
            .WithSummary("Get dashboard blog data (Admin only)")
            .WithDescription(
                "Returns the latest published non-deleted blogs (id, title, author name, created date) " +
                "and monthly blog counts for the past N months to support frontend charts.")
            .Produces<GetDashboardBlogsResponse>(200)
            .RequireAuthorization("AdminOnly");
    }
}
