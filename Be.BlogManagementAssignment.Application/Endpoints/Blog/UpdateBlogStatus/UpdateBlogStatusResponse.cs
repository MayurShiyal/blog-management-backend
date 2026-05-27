namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;
public class UpdateBlogStatusResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateBlogStatusDto? Data { get; set; }
}
