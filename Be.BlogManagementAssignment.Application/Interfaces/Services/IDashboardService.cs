using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardSummary;

namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<GetDashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<GetDashboardStatusResponse> GetStatusAsync(CancellationToken cancellationToken = default);

    Task<GetDashboardBlogsResponse> GetBlogsAsync(
        int latestCount,
        int monthsBack,
        CancellationToken cancellationToken = default);
}
