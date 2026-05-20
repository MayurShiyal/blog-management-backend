namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;

public class GetBlogByIdResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public GetBlogByIdDto? Data { get; set; }
}
