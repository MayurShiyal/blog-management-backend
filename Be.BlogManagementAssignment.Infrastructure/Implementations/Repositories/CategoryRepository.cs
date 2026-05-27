using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

/// <summary>
/// EF Core + PostgreSQL implementation of <see cref="ICategoryRepository"/>.
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Category> Items, int TotalCount, int ActiveCount, int InactiveCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = _context.Categories
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            baseQuery = baseQuery.Where(c => EF.Functions.ILike(c.Name, $"%{term}%"));
        }

        int activeCount = await baseQuery.CountAsync(c => c.IsActive, cancellationToken);
        int inactiveCount = await baseQuery.CountAsync(c => !c.IsActive, cancellationToken);
        int totalCount = activeCount + inactiveCount;

        var filteredQuery = isActive.HasValue
            ? baseQuery.Where(c => c.IsActive == isActive.Value)
            : baseQuery;

        var items = await filteredQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount, activeCount, inactiveCount);
    }

    /// <inheritdoc />
    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _context.Categories
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var lower = name.Trim().ToLowerInvariant();
        return _context.Categories.AnyAsync(
            c => !c.IsDeleted && c.Name.ToLower() == lower && (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsBySlugAsync(
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var lower = slug.Trim().ToLowerInvariant();
        return _context.Categories.AnyAsync(
            c => !c.IsDeleted && c.Slug.ToLower() == lower && (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasBlogsAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.BlogCategories
            .AnyAsync(bc => bc.CategoryId == categoryId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
