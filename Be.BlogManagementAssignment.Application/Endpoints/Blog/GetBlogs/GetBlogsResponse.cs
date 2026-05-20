namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;

public class GetBlogsResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public IEnumerable<BlogListItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
