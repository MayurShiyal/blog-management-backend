namespace Be.BlogManagementAssignment.Application.Endpoints.Category.ActiveCategories;

public class GetActiveCategoriesResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public IEnumerable<GetActiveCategoriesItemDto>? Data { get; set; } //actual payload
}
