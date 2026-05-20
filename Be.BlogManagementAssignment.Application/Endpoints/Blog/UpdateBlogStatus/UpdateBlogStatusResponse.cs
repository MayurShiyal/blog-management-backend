namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

/// <summary>Response envelope for PATCH /api/blogs/{id}/status.</summary>
public class UpdateBlogStatusResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateBlogStatusDto? Data { get; set; }
}
