using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardSummary;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<GetDashboardSummaryResponse> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalUsers      = await _dashboardRepository.GetTotalUsersAsync(cancellationToken);
        var totalBlogs      = await _dashboardRepository.GetTotalBlogsAsync(cancellationToken);
        var totalCategories = await _dashboardRepository.GetTotalCategoriesAsync(cancellationToken);

        return new GetDashboardSummaryResponse
        {
            Status  = true,
            Message = "Dashboard summary retrieved successfully.",
            Data    = new DashboardSummaryDto
            {
                TotalUsers      = totalUsers,
                TotalBlogs      = totalBlogs,
                TotalCategories = totalCategories
            }
        };
    }

    public async Task<GetDashboardStatusResponse> GetStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var (activeUsers,   inactiveUsers)      = await _dashboardRepository.GetUserStatusCountsAsync(cancellationToken);
        var (activeCategories, inactiveCategories) = await _dashboardRepository.GetCategoryStatusCountsAsync(cancellationToken);

        return new GetDashboardStatusResponse
        {
            Status  = true,
            Message = "Dashboard status retrieved successfully.",
            Data    = new DashboardStatusDto
            {
                Users = new UserStatusCountsDto
                {
                    Active   = activeUsers,
                    Inactive = inactiveUsers
                },
                Categories = new CategoryStatusCountsDto
                {
                    Active   = activeCategories,
                    Inactive = inactiveCategories
                }
            }
        };
    }

    public async Task<GetDashboardBlogsResponse> GetBlogsAsync(
        int latestCount,
        int monthsBack,
        CancellationToken cancellationToken = default)
    {
        latestCount = latestCount < 1 ? 5  : (latestCount > 50 ? 50 : latestCount);
        monthsBack  = monthsBack  < 1 ? 12 : (monthsBack  > 24 ? 24 : monthsBack);

        var latestBlogs        = await _dashboardRepository.GetLatestPublishedBlogsAsync(latestCount, cancellationToken);
        var monthlyBlogCounts  = await _dashboardRepository.GetMonthlyBlogCountsAsync(monthsBack, cancellationToken);

        return new GetDashboardBlogsResponse
        {
            Status  = true,
            Message = "Dashboard blog data retrieved successfully.",
            Data    = new DashboardBlogsDto
            {
                LatestBlogs = latestBlogs.Select(b => new LatestBlogItemDto
                {
                    Id         = b.Id,
                    Title      = b.Title,
                    AuthorName = b.AuthorFullName,
                    CreatedAt  = b.CreatedAt
                }),
                MonthlyBlogCounts = monthlyBlogCounts.Select(m => new MonthlyBlogCountDto
                {
                    Month = m.Month,
                    Count = m.Count
                })
            }
        };
    }
}
