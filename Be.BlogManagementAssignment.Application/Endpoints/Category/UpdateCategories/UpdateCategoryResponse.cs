namespace Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;

public class UpdateCategoryResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public UpdateCategoryDto? Data { get; set; }
}
