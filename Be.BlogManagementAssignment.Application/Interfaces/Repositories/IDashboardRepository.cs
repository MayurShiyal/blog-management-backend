namespace Be.BlogManagementAssignment.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<int> GetTotalUsersAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalBlogsAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalCategoriesAsync(CancellationToken cancellationToken = default);

    Task<(int Active, int Inactive)> GetUserStatusCountsAsync(CancellationToken cancellationToken = default);

    Task<(int Active, int Inactive)> GetCategoryStatusCountsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid Id, string Title, string AuthorFullName, DateTime CreatedAt)>> GetLatestPublishedBlogsAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<(string Month, int Count)>> GetMonthlyBlogCountsAsync(
        int monthsBack,
        CancellationToken cancellationToken = default);
}
