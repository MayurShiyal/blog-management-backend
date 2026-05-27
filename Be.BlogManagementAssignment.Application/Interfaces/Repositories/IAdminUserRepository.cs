using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Interfaces.Repositories;

public interface IAdminUserRepository
{
    Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(
        string? search,
        UserRole? role,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
