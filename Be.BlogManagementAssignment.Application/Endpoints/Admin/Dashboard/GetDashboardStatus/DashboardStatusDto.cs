namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.Dashboard.GetDashboardStatus;

public class UserStatusCountsDto
{
    public int Active { get; set; }
    public int Inactive { get; set; }
}

public class CategoryStatusCountsDto
{
    public int Active { get; set; }
    public int Inactive { get; set; }
}

public class DashboardStatusDto
{
    public UserStatusCountsDto Users { get; set; } = default!;
    public CategoryStatusCountsDto Categories { get; set; } = default!;
}
