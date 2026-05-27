using Be.BlogManagementAssignment.Application.Endpoints.Admin.DeleteUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUserById;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;
using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Services;

public sealed class AdminUserService : IAdminUserService
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly ILogger<AdminUserService> _logger;

    public AdminUserService(
        IAdminUserRepository adminUserRepository,
        ILogger<AdminUserService> logger)
    {
        _adminUserRepository = adminUserRepository;
        _logger = logger;
    }

    public async Task<GetUsersResponse> GetUsersAsync(
        string? search,
        UserRole? role,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _adminUserRepository.GetAllAsync(
            search, role, status, pageNumber, pageSize, cancellationToken);

        return new GetUsersResponse
        {
            Status = true,
            Message = "Users retrieved successfully.",
            Items = items.Select(MapToUserListItemDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<GetUserByIdResponse> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _adminUserRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        return new GetUserByIdResponse
        {
            Status = true,
            Message = "User retrieved successfully.",
            Data = MapToGetUserByIdDto(user)
        };
    }

    public async Task<UpdateUserResponse> UpdateUserAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _adminUserRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        existing.FirstName = request.FirstName.Trim();
        existing.LastName = request.LastName?.Trim();
        existing.UpdatedAt = DateTime.UtcNow;

        await _adminUserRepository.UpdateAsync(existing, cancellationToken);

        _logger.LogInformation("User updated: {Email} (id={Id})", existing.Email, existing.Id);

        return new UpdateUserResponse
        {
            Status = true,
            Message = "User updated successfully.",
            Data = MapToUpdateUserDto(existing)
        };
    }

    public async Task<UpdateUserStatusResponse> UpdateUserStatusAsync(
        Guid id,
        UpdateUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _adminUserRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        if (existing.Status == request.Status)
            throw new AppException($"User status is already {request.Status}.", 400);

        existing.Status = request.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        await _adminUserRepository.UpdateAsync(existing, cancellationToken);

        _logger.LogInformation(
            "User status updated to {Status}: {Email} (id={Id})",
            existing.Status, existing.Email, existing.Id);

        return new UpdateUserStatusResponse
        {
            Status = true,
            Message = $"User status updated to {existing.Status} successfully.",
            Data = MapToUpdateUserStatusDto(existing)
        };
    }

    public async Task<DeleteUserResponse> DeleteUserAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _adminUserRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        existing.IsDeleted = true;
        existing.DeletedAt = DateTime.UtcNow;
        existing.UpdatedAt = DateTime.UtcNow;

        await _adminUserRepository.UpdateAsync(existing, cancellationToken);

        _logger.LogInformation("User soft-deleted: {Email} (id={Id})", existing.Email, existing.Id);

        return new DeleteUserResponse
        {
            Status = true,
            Message = "User deleted successfully."
        };
    }

    private static string BuildFullName(User u) =>
        $"{u.FirstName} {u.LastName}".Trim();

    private static UserListItemDto MapToUserListItemDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        FullName = BuildFullName(u),
        Email = u.Email,
        Role = u.Role.ToString(),
        Status = u.Status.ToString(),
        IsVerified = u.IsVerified,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };

    private static GetUserByIdDto MapToGetUserByIdDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        FullName = BuildFullName(u),
        Email = u.Email,
        Role = u.Role.ToString(),
        Status = u.Status.ToString(),
        IsVerified = u.IsVerified,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };

    private static UpdateUserDto MapToUpdateUserDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        FullName = BuildFullName(u),
        Email = u.Email,
        Role = u.Role.ToString(),
        Status = u.Status.ToString(),
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };

    private static UpdateUserStatusDto MapToUpdateUserStatusDto(User u) => new()
    {
        Id = u.Id,
        FullName = BuildFullName(u),
        Email = u.Email,
        Status = u.Status.ToString(),
        UpdatedAt = u.UpdatedAt
    };
}
