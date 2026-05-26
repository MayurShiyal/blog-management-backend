using Be.BlogManagementAssignment.Domain.Entities;

namespace Be.BlogManagementAssignment.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Returns a paginated list of categories with global active/inactive counts.
    /// Active/inactive counts reflect ALL matching rows (not just the current page).
    /// </summary>
    Task<(IEnumerable<Category> Items, int TotalCount, int ActiveCount, int InactiveCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the category with the given id, or null.</summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Returns all categories where IsActive = true.</summary>
    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns true when a category with the given name already exists (case-insensitive).</summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Returns true when a category with the given slug already exists (case-insensitive).</summary>
    Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Persists a new category and returns the saved entity.</summary>
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> HasBlogsAsync(int categoryId, CancellationToken cancellationToken = default);
    /// <summary>Persists changes to an existing category entity.</summary>
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>Deletes the category. Throws if blogs are linked.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
