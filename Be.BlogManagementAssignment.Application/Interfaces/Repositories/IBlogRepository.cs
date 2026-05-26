using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Interfaces.Repositories;

public interface IBlogRepository
{
    /// <summary>Returns a paginated list of all blogs (Admin view).</summary>
    Task<(IEnumerable<Blog> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        BlogStatus? status,
        int? categoryId,
        Guid? authorId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a paginated list of published blogs (public).</summary>
    Task<(IEnumerable<Blog> Items, int TotalCount)> GetPublishedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        int? categoryId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a paginated list of blogs belonging to a specific author.</summary>
    Task<(IEnumerable<Blog> Items, int TotalCount)> GetByAuthorAsync(
        Guid authorId,
        int pageNumber,
        int pageSize,
        string? search,
        BlogStatus? status,
        int? categoryId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a blog by its ID, or null if not found.</summary>
    Task<Blog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns a blog by its slug (any status), or null if not found.</summary>
    Task<Blog?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Returns true when a blog with the given slug already exists.</summary>
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Persists a new blog and returns the saved entity.</summary>
    Task<Blog> CreateAsync(Blog blog, CancellationToken cancellationToken = default);

    /// <summary>Persists changes to an existing blog entity.</summary>
    Task UpdateAsync(Blog blog, CancellationToken cancellationToken = default);

    /// <summary>Deletes the blog by ID.</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
