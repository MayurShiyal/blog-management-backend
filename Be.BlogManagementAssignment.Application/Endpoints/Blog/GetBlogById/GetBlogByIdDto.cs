namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;

public class GetBlogByIdDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public string Status { get; set; } = default!;
    public string? RejectionReason { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public List<string> CategoryNames { get; set; } = new();
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
