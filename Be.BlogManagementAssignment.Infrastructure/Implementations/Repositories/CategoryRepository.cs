using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ICategoryRepository"/>.
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c => c.Name.ToLower().Contains(term));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Category>>(
            _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .AsEnumerable());

    // Use an async version for the active list so it properly awaits EF
    public async Task<IEnumerable<Category>> GetActiveAsyncReal(CancellationToken cancellationToken = default)
        => await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var lower = name.Trim().ToLowerInvariant();
        return _context.Categories.AnyAsync(
            c => c.Name.ToLower() == lower && (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var lower = slug.Trim().ToLowerInvariant();
        return _context.Categories.AnyAsync(
            c => c.Slug.ToLower() == lower && (excludeId == null || c.Id != excludeId),
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
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        var hasBlogs = await _context.Blogs.AnyAsync(b => b.CategoryId == id, cancellationToken);
        if (hasBlogs)
            throw new AppException("Cannot delete a category that has blogs assigned to it.", 409);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
