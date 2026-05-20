namespace Be.BlogManagementAssignment.Application.Endpoints.Category.CategoriesById;

public class GetCategoryByIdResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public GetCategoryByIdDto? Data { get; set; }
}
