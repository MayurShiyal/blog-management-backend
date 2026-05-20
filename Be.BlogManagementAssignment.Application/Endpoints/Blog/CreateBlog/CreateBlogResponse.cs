namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public class CreateBlogResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public CreateBlogDto? Data { get; set; }
}
