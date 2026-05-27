namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;

public class GetUsersResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public IEnumerable<UserListItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
