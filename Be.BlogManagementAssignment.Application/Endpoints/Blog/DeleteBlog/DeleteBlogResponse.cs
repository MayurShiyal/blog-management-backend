namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;

/// <summary>
/// Response envelope returned by DELETE /api/blogs/{id}.
/// </summary>
public class DeleteBlogResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
}
