namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardBlogs;

public class LatestBlogItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string AuthorName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class MonthlyBlogCountDto
{
    public string Month { get; set; } = default!;
    public int Count { get; set; }
}

public class DashboardBlogsDto
{
    public IEnumerable<LatestBlogItemDto> LatestBlogs { get; set; } = [];
    public IEnumerable<MonthlyBlogCountDto> MonthlyBlogCounts { get; set; } = [];
}
