using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;
public class UpdateUserStatusRequest
{
    public UserStatus Status { get; set; }
}
