namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

/// <summary>
/// PUT /api/blogs/{id}
///
/// Updates blog content fields (title, slug, short description, content,
/// thumbnail, category). Authors may save a draft or submit for approval
/// by calling PATCH /api/blogs/{id}/status separately.
/// </summary>
public class UpdateBlogRequest
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? CategoryId { get; set; }
}
