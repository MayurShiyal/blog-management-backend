using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public class CreateBlogRequest
{
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }

    /// <summary>One or more category IDs the blog should belong to.</summary>
    public List<int> CategoryIds { get; set; } = new();

    public BlogStatus Status { get; set; }
}
