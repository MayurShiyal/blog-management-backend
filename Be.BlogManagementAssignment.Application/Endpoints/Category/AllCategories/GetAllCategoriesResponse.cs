namespace Be.BlogManagementAssignment.Application.Endpoints.Category.AllCategories;

public class GetAllCategoriesResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public IEnumerable<GetAllCategoriesItemDto> Items { get; set; } = Enumerable.Empty<GetAllCategoriesItemDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
