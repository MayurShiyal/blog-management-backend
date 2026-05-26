namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public class CreateBlogDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public int Status { get; set; }

    /// <summary>IDs of all categories this blog belongs to.</summary>
    public List<int> CategoryIds { get; set; } = new();

    /// <summary>Names of all categories this blog belongs to.</summary>
    public List<string> CategoryNames { get; set; } = new();

    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
