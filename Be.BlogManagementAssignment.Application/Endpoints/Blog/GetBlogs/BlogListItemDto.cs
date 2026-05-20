namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;

public class BlogListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public string Status { get; set; } = default!;
    public string? RejectionReason { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
