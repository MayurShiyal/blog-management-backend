using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

/// <summary>EF Core implementation of <see cref="IBlogRepository"/>.</summary>
public sealed class BlogRepository : IBlogRepository
{
    private readonly ApplicationDbContext _context;

    public BlogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Blog> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        BlogStatus? status,
        int? categoryId,
        Guid? authorId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Blogs
            .AsNoTracking()
            .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(b => b.Title.ToLower().Contains(term) || b.Slug.ToLower().Contains(term));
        }

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (categoryId.HasValue)
        {
            if (categoryId.Value == -1)
                // "General" filter: blogs that have more than one category assigned
                query = query.Where(b => b.BlogCategories.Count > 1);
            else
                // Specific category filter: any blog that includes this category (single or multi)
                query = query.Where(b => b.BlogCategories.Any(bc => bc.CategoryId == categoryId.Value));
        }

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        int totalCount = await query.CountAsync(cancellationToken);

        query = ApplySort(query, sortBy, sortDesc);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Blog> Items, int TotalCount)> GetPublishedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        int? categoryId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Blogs
            .AsNoTracking()
            .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Author)
            .Where(b => b.Status == BlogStatus.Published);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(b => b.Title.ToLower().Contains(term)
                                  || b.ShortDescription!.ToLower().Contains(term));
        }

        if (categoryId.HasValue)
        {
            if (categoryId.Value == -1)
                // "General" filter: blogs that have more than one category assigned
                query = query.Where(b => b.BlogCategories.Count > 1);
            else
                // Specific category filter: any blog that includes this category (single or multi)
                query = query.Where(b => b.BlogCategories.Any(bc => bc.CategoryId == categoryId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Default sort for public feed: most recently published first
        query = ApplySort(query, sortBy ?? "publishedAt", sortDesc);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Blog> Items, int TotalCount)> GetByAuthorAsync(
        Guid authorId,
        int pageNumber,
        int pageSize,
        string? search,
        BlogStatus? status,
        int? categoryId,
        string? sortBy,
        bool sortDesc,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Blogs
            .AsNoTracking()
            .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Author)
            .Where(b => b.AuthorId == authorId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(b => b.Title.ToLower().Contains(term));
        }

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (categoryId.HasValue)
        {
            if (categoryId.Value == -1)
                // "General" filter: blogs that have more than one category assigned
                query = query.Where(b => b.BlogCategories.Count > 1);
            else
                // Specific category filter: any blog that includes this category (single or multi)
                query = query.Where(b => b.BlogCategories.Any(bc => bc.CategoryId == categoryId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySort(query, sortBy, sortDesc);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public Task<Blog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Blogs
            .AsNoTracking()
            .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<Blog?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => _context.Blogs
            .AsNoTracking()
            .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(
                b => b.Slug == slug.Trim().ToLowerInvariant(),
                cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var lower = slug.Trim().ToLowerInvariant();
        return _context.Blogs.AnyAsync(
            b => b.Slug == lower && (excludeId == null || b.Id != excludeId),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Blog> CreateAsync(Blog blog, CancellationToken cancellationToken = default)
    {
        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync(cancellationToken);
        return blog;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Blog blog, CancellationToken cancellationToken = default)
    {
        // Re-attach in tracking mode so EF can detect changes to BlogCategories
        var tracked = await _context.Blogs
            .Include(b => b.BlogCategories)
            .FirstOrDefaultAsync(b => b.Id == blog.Id, cancellationToken)
            ?? throw new NotFoundException($"Blog with id {blog.Id} was not found.");

        // Scalar fields
        tracked.Title = blog.Title;
        tracked.Slug = blog.Slug;
        tracked.ShortDescription = blog.ShortDescription;
        tracked.Content = blog.Content;
        tracked.ThumbnailUrl = blog.ThumbnailUrl;
        tracked.Status = blog.Status;
        tracked.RejectionReason = blog.RejectionReason;
        tracked.PublishedAt = blog.PublishedAt;
        tracked.UpdatedAt = blog.UpdatedAt;
        tracked.AuthorId = blog.AuthorId;

        // Replace category links
        tracked.BlogCategories.Clear();
        foreach (var bc in blog.BlogCategories)
            tracked.BlogCategories.Add(new BlogCategory { BlogId = tracked.Id, CategoryId = bc.CategoryId });

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var blog = await _context.Blogs.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException($"Blog with id {id} was not found.");

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Sorting ──────────────────────────────────────────────────────────────

    private static IQueryable<Blog> ApplySort(IQueryable<Blog> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "title" => sortDesc ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "updatedat" => sortDesc ? query.OrderByDescending(b => b.UpdatedAt) : query.OrderBy(b => b.UpdatedAt),
            "publishedat" => sortDesc ? query.OrderByDescending(b => b.PublishedAt) : query.OrderBy(b => b.PublishedAt),
            "status" => sortDesc ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status),
            _ => sortDesc ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt)
        };
    }
}
