namespace Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;

public class CreateCategoryRequest
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
