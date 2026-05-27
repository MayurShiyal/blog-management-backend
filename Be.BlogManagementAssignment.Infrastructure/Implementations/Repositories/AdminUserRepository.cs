using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

public sealed class AdminUserRepository : IAdminUserRepository
{
    private readonly ApplicationDbContext _context;

    public AdminUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(
        string? search,
        UserRole? role,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users
     .Where(u =>
         !u.IsDeleted &&
         u.Role != UserRole.Admin).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u =>
                EF.Functions.ILike(u.FirstName, $"%{term}%") ||
                (u.LastName != null && EF.Functions.ILike(u.LastName, $"%{term}%")) ||
                EF.Functions.ILike(u.Email, $"%{term}%"));
        }

        if (role.HasValue)
            query = query.Where(u => u.Role == role.Value);

        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users
            .FirstOrDefaultAsync(
    u => u.Id == id &&
         !u.IsDeleted &&
         u.Role != UserRole.Admin,
    cancellationToken);

    /// <inheritdoc />
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
