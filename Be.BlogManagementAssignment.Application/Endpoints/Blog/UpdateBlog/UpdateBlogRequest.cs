using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

/// <summary>
/// PUT /api/blogs/{id}
///
/// Updates blog content fields (title, slug, short description, content,
/// thumbnail, categories). Authors may also transition a Draft blog to
/// PendingApproval by setting Status = PendingApproval in the same request.
/// </summary>
public class UpdateBlogRequest
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// When provided, replaces the blog's entire category list.
    /// Must contain at least one valid category ID.
    /// </summary>
    public List<int>? CategoryIds { get; set; }

    /// <summary>
    /// Optional status transition. Authors may set this to PendingApproval
    /// to submit a Draft blog for review without a separate PATCH call.
    /// Only Draft → PendingApproval is permitted for authors.
    /// </summary>
    public BlogStatus? Status { get; set; }
}
