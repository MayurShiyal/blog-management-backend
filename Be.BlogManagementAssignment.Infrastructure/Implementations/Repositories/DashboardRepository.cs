using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Enums;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

/// <summary>EF Core implementation of <see cref="IDashboardRepository"/>.</summary>
public sealed class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<int> GetTotalUsersAsync(CancellationToken cancellationToken = default)
        => _context.Users
            .CountAsync(u => !u.IsDeleted && u.Role != UserRole.Admin, cancellationToken);

    /// <inheritdoc />
    public Task<int> GetTotalBlogsAsync(CancellationToken cancellationToken = default)
        => _context.Blogs
            .CountAsync(b => !b.IsDeleted, cancellationToken);

    /// <inheritdoc />
    public Task<int> GetTotalCategoriesAsync(CancellationToken cancellationToken = default)
        => _context.Categories
            .CountAsync(c => !c.IsDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<(int Active, int Inactive)> GetUserStatusCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var counts = await _context.Users
            .Where(u => !u.IsDeleted && u.Role != UserRole.Admin)
            .GroupBy(u => u.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int active   = counts.FirstOrDefault(x => x.Status == UserStatus.Active)?.Count   ?? 0;
        int inactive = counts.FirstOrDefault(x => x.Status == UserStatus.Inactive)?.Count ?? 0;

        return (active, inactive);
    }

    /// <inheritdoc />
    public async Task<(int Active, int Inactive)> GetCategoryStatusCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var counts = await _context.Categories
            .Where(c => !c.IsDeleted)
            .GroupBy(c => c.IsActive)
            .Select(g => new { IsActive = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int active   = counts.FirstOrDefault(x =>  x.IsActive)?.Count ?? 0;
        int inactive = counts.FirstOrDefault(x => !x.IsActive)?.Count ?? 0;

        return (active, inactive);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(Guid Id, string Title, string AuthorFullName, DateTime CreatedAt)>>
        GetLatestPublishedBlogsAsync(int count, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Blogs
            .Where(b => !b.IsDeleted && b.Status == BlogStatus.Published)
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .Select(b => new
            {
                b.Id,
                b.Title,
                AuthorFirstName = b.Author.FirstName,
                AuthorLastName  = b.Author.LastName,
                b.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return rows.Select(r => (
            r.Id,
            r.Title,
            AuthorFullName: $"{r.AuthorFirstName} {r.AuthorLastName}".Trim(),
            r.CreatedAt));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(string Month, int Count)>> GetMonthlyBlogCountsAsync(
        int monthsBack, CancellationToken cancellationToken = default)
    {
        var from = DateTime.UtcNow.AddMonths(-monthsBack + 1);
        var fromStartOfMonth = new DateTime(from.Year, from.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var rows = await _context.Blogs
            .Where(b => !b.IsDeleted && b.CreatedAt >= fromStartOfMonth)
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToListAsync(cancellationToken);

        return rows.Select(r => (
            Month: $"{r.Year:D4}-{r.Month:D2}",
            r.Count));
    }
}
