using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;
public class UpdateBlogStatusRequest
{
    public BlogStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}
