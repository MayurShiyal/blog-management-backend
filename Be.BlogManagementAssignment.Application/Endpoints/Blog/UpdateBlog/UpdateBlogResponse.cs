namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

public class UpdateBlogResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateBlogDto? Data { get; set; }
}
