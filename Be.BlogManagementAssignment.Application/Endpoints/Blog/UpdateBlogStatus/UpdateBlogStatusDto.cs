namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

/// <summary>Lightweight status-change result DTO (used internally by UpdateBlogAsync).</summary>
public class UpdateBlogStatusDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? RejectionReason { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}