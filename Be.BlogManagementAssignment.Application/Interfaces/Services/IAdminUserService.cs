using Be.BlogManagementAssignment.Application.Endpoints.Admin.DeleteUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUserById;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;
using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface IAdminUserService
{
    Task<GetUsersResponse> GetUsersAsync(
        string? search,
        UserRole? role,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<GetUserByIdResponse> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<UpdateUserResponse> UpdateUserAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateUserStatusResponse> UpdateUserStatusAsync(
        Guid id,
        UpdateUserStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteUserResponse> DeleteUserAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
