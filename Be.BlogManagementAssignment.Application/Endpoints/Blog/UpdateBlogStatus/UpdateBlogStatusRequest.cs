using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

/// <summary>
/// PATCH /api/blogs/{id}/status
///
/// Used by Admin to approve or reject a blog.
///   Status = Published (2) → approve
///   Status = Rejected  (3) → reject (RejectionReason required)
/// </summary>
public class UpdateBlogStatusRequest
{
    /// <summary>Target status. Must be Published or Rejected.</summary>
    public BlogStatus Status { get; set; }

    /// <summary>Required when Status = Rejected.</summary>
    public string? RejectionReason { get; set; }
}
