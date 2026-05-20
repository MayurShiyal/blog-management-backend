namespace Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;

public class CreateCategoryResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public CreateCategoryDto? Data { get; set; }
}
