namespace Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
