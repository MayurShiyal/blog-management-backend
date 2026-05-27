using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

public class UpdateBlogRequest
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<int>? CategoryIds { get; set; }
    public BlogStatus? Status { get; set; }
}
