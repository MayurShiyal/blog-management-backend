namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

public class UpdateBlogStatusDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? RejectionReason { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}