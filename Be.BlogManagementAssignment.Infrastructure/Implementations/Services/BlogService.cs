using Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;
using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Services;

public sealed class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<BlogService> _logger;

    public BlogService(
        IBlogRepository blogRepository,
        ICategoryRepository categoryRepository,
        ILogger<BlogService> logger)
    {
        _blogRepository = blogRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    // ── GET /api/blogs ───────────────────────────────────────────────────────

    public async Task<GetBlogsResponse> GetBlogsAsync(
        string? role,
        string? userId,
        string? slug,
        BlogStatus? status,
        string? search,
        bool? mine,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        int? categoryId,
        CancellationToken cancellationToken = default)
    {
        (pageNumber, pageSize) = Paginate(pageNumber, pageSize);

        if (!string.IsNullOrWhiteSpace(slug))
        {
            var bySlug = await _blogRepository.GetBySlugAsync(slug, cancellationToken);
            if (bySlug is null)
                return EmptyListResponse("No blog found with the given slug.");

            if (role != "Admin" && bySlug.Status != BlogStatus.Published)
            {
                if (role != "Author" || !Guid.TryParse(userId, out var callerId) || bySlug.AuthorId != callerId)
                    return EmptyListResponse("No blog found with the given slug.");
            }

            return BuildListResponse("Blog retrieved by slug.", [bySlug], 1, 1, 1);
        }

        if (role == "Admin")
        {
            var (items, total) = await _blogRepository.GetAllAsync(
                pageNumber, pageSize, search, status, categoryId, sortBy, sortDesc, cancellationToken);
            return BuildListResponse("Blogs retrieved successfully.", items, total, pageNumber, pageSize);
        }

        if (role == "Author" && Guid.TryParse(userId, out var authorId))
        {
            bool showMine = mine ?? true;
            if (showMine)
            {
                var (items, total) = await _blogRepository.GetByAuthorAsync(
                    authorId, pageNumber, pageSize, search, status, sortBy, sortDesc, cancellationToken);
                return BuildListResponse("Blogs retrieved successfully.", items, total, pageNumber, pageSize);
            }
        }

        {
            var (items, total) = await _blogRepository.GetPublishedAsync(
                pageNumber, pageSize, search, categoryId, sortBy, sortDesc, cancellationToken);
            return BuildListResponse("Published blogs retrieved successfully.", items, total, pageNumber, pageSize);
        }
    }

    // ── GET /api/blogs/{id} ──────────────────────────────────────────────────

    public async Task<GetBlogByIdResponse> GetBlogByIdAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var blog = await _blogRepository.GetByIdAsync(blogId, cancellationToken)
            ?? throw new NotFoundException($"Blog with id {blogId} was not found.");

        if (role == "Admin")
            return OkBlogByIdResponse(blog);

        if (role == "Author" && Guid.TryParse(userId, out var authorId))
        {
            if (blog.AuthorId != authorId)
                throw new AppException("You are not authorized to view this blog.", 403);
            return OkBlogByIdResponse(blog);
        }

        if (blog.Status != BlogStatus.Published)
            throw new NotFoundException($"Blog with id {blogId} was not found.");

        return OkBlogByIdResponse(blog);
    }

    // ── POST /api/blogs ──────────────────────────────────────────────────────

    public async Task<CreateBlogResponse> CreateAsync(
        CreateBlogRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _blogRepository.ExistsBySlugAsync(slug, cancellationToken: cancellationToken))
            throw new ConflictException($"A blog with the slug '{slug}' already exists.");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category with id {request.CategoryId} was not found.");

        if (!category.IsActive)
            throw new AppException("The selected category is not active.", 400);

        var blog = new Blog
        {
            Title            = request.Title.Trim(),
            Slug             = slug,
            ShortDescription = request.ShortDescription?.Trim(),
            Content          = request.Content.Trim(),
            ThumbnailUrl     = request.ThumbnailUrl?.Trim(),
            Status           = BlogStatus.PendingApproval,
            CategoryId       = request.CategoryId,
            AuthorId         = authorId,
            CreatedAt        = DateTime.UtcNow
        };

        var created = await _blogRepository.CreateAsync(blog, cancellationToken);
        created.Category = category;

        _logger.LogInformation("Blog created: {Title} (id={Id}) by author {AuthorId}", created.Title, created.Id, authorId);

        return new CreateBlogResponse
        {
            Status  = true,
            Message = "Blog created successfully.",
            Data    = MapToCreateBlogDto(created)
        };
    }

    // ── PUT /api/blogs/{id} ──────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<UpdateBlogResponse> UpdateBlogAsync(
        Guid blogId,
        UpdateBlogRequest request,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _blogRepository.GetByIdAsync(blogId, cancellationToken)
            ?? throw new NotFoundException($"Blog with id {blogId} was not found.");

        // Authors may only edit their own blogs; Admins may edit any
        if (role != "Admin")
        {
            if (role != "Author" || !Guid.TryParse(userId, out var authorId) || existing.AuthorId != authorId)
                throw new AppException("You are not authorized to update this blog.", 403);
        }

        // Only Draft or Rejected blogs can be edited
        if (existing.Status != BlogStatus.Draft && existing.Status != BlogStatus.Rejected)
            throw new AppException("Only Draft or Rejected blogs can be updated.", 400);

        await ApplyContentUpdatesAsync(existing, request, cancellationToken);
        existing.UpdatedAt = DateTime.UtcNow;

        await _blogRepository.UpdateAsync(existing, cancellationToken);
        _logger.LogInformation("Blog updated: {Id}", existing.Id);

        return new UpdateBlogResponse
        {
            Status  = true,
            Message = "Blog updated successfully.",
            Data    = MapToUpdateBlogDto(existing)
        };
    }

    // ── PATCH /api/blogs/{id}/status ─────────────────────────────────────────

    /// <inheritdoc />
    public async Task<UpdateBlogStatusResponse> UpdateBlogStatusAsync(
        Guid blogId,
        UpdateBlogStatusRequest request,
        string? role,
        CancellationToken cancellationToken = default)
    {
        AssertIsAdmin(role, "change blog status");

        var existing = await _blogRepository.GetByIdAsync(blogId, cancellationToken)
            ?? throw new NotFoundException($"Blog with id {blogId} was not found.");

        return request.Status switch
        {
            BlogStatus.Published => await HandleApproveAsync(existing, cancellationToken),
            BlogStatus.Rejected  => await HandleRejectAsync(existing, request, cancellationToken),
            _                    => throw new AppException(
                $"Status must be Published (approve) or Rejected (reject). Got '{request.Status}'.", 400)
        };
    }

    // ── DELETE /api/blogs/{id} ───────────────────────────────────────────────

    public async Task<DeleteBlogResponse> DeleteBlogAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _blogRepository.GetByIdAsync(blogId, cancellationToken)
            ?? throw new NotFoundException($"Blog with id {blogId} was not found.");

        if (role == "Admin")
        {
            await _blogRepository.DeleteAsync(blogId, cancellationToken);
            _logger.LogInformation("Blog deleted by admin: {Title} (id={Id})", existing.Title, blogId);
            return new DeleteBlogResponse { Status = true, Message = "Blog deleted successfully." };
        }

        if (role == "Author" && Guid.TryParse(userId, out var authorId))
        {
            if (existing.AuthorId != authorId)
                throw new AppException("You are not authorized to delete this blog.", 403);

            await _blogRepository.DeleteAsync(blogId, cancellationToken);
            _logger.LogInformation("Blog deleted by author: {Title} (id={Id})", existing.Title, blogId);
            return new DeleteBlogResponse { Status = true, Message = "Blog deleted successfully." };
        }

        throw new AppException("You are not authorized to delete this blog.", 403);
    }

    // ── Private status transition handlers ───────────────────────────────────

    private async Task<UpdateBlogStatusResponse> HandleApproveAsync(Blog existing, CancellationToken ct)
    {
        if (existing.Status != BlogStatus.PendingApproval)
            throw new AppException("Only PendingApproval blogs can be approved.", 400);

        existing.Status          = BlogStatus.Published;
        existing.RejectionReason = null;
        existing.PublishedAt     = DateTime.UtcNow;
        existing.UpdatedAt       = DateTime.UtcNow;

        await _blogRepository.UpdateAsync(existing, ct);
        _logger.LogInformation("Blog approved and published: {Id}", existing.Id);

        return OkStatusResponse("Blog approved and published successfully.", existing);
    }

    private async Task<UpdateBlogStatusResponse> HandleRejectAsync(
        Blog existing,
        UpdateBlogStatusRequest request,
        CancellationToken ct)
    {
        if (existing.Status != BlogStatus.PendingApproval)
            throw new AppException("Only PendingApproval blogs can be rejected.", 400);

        if (string.IsNullOrWhiteSpace(request.RejectionReason))
            throw new AppException("RejectionReason is required when rejecting a blog.", 400);

        existing.Status          = BlogStatus.Rejected;
        existing.RejectionReason = request.RejectionReason.Trim();
        existing.UpdatedAt       = DateTime.UtcNow;

        await _blogRepository.UpdateAsync(existing, ct);
        _logger.LogInformation("Blog rejected: {Id}. Reason: {Reason}", existing.Id, existing.RejectionReason);

        return OkStatusResponse($"Blog rejected. Reason: {existing.RejectionReason}", existing);
    }

    // ── Shared content update helper ─────────────────────────────────────────

    private async Task ApplyContentUpdatesAsync(Blog existing, UpdateBlogRequest request, CancellationToken ct)
    {
        if (request.Title is not null)            existing.Title            = request.Title.Trim();
        if (request.ShortDescription is not null) existing.ShortDescription = request.ShortDescription.Trim();
        if (request.Content is not null)          existing.Content          = request.Content.Trim();
        if (request.ThumbnailUrl is not null)     existing.ThumbnailUrl     = request.ThumbnailUrl.Trim();

        if (request.Slug is not null)
        {
            var slug = request.Slug.Trim().ToLowerInvariant();
            if (await _blogRepository.ExistsBySlugAsync(slug, excludeId: existing.Id, cancellationToken: ct))
                throw new ConflictException($"A blog with the slug '{slug}' already exists.");
            existing.Slug = slug;
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, ct)
                ?? throw new NotFoundException($"Category with id {request.CategoryId.Value} was not found.");
            if (!category.IsActive)
                throw new AppException("The selected category is not active.", 400);
            existing.CategoryId = request.CategoryId.Value;
        }
    }

    // ── Guard helpers ────────────────────────────────────────────────────────

    private static void AssertIsAdmin(string? role, string action)
    {
        if (role != "Admin")
            throw new AppException($"Only Admins are allowed to {action}.", 403);
    }

    // ── Pagination ───────────────────────────────────────────────────────────

    private static (int pageNumber, int pageSize) Paginate(int pageNumber, int pageSize) =>
        (pageNumber < 1 ? 1 : pageNumber,
         pageSize   < 1 ? 10 : pageSize > 100 ? 100 : pageSize);

    // ── Response builders ────────────────────────────────────────────────────

    private static GetBlogsResponse EmptyListResponse(string message) => new()
    {
        Status = false, Message = message, Items = [], TotalCount = 0, PageNumber = 1, PageSize = 0
    };

    private static GetBlogsResponse BuildListResponse(
        string message,
        IEnumerable<Blog> items,
        int totalCount,
        int pageNumber,
        int pageSize) => new()
    {
        Status     = true,
        Message    = message,
        Items      = items.Select(MapToBlogListItemDto),
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize   = pageSize
    };

    private static GetBlogByIdResponse OkBlogByIdResponse(Blog b) => new()
    {
        Status  = true,
        Message = "Blog retrieved successfully.",
        Data    = MapToGetBlogByIdDto(b)
    };

    private static UpdateBlogStatusResponse OkStatusResponse(string message, Blog b) => new()
    {
        Status  = true,
        Message = message,
        Data    = MapToUpdateBlogStatusDto(b)
    };

    // ── Mappers ──────────────────────────────────────────────────────────────

    private static string? GetAuthorName(Blog b) =>
        b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}".Trim() : null;

    private static BlogListItemDto MapToBlogListItemDto(Blog b) => new()
    {
        Id               = b.Id,
        Title            = b.Title,
        Slug             = b.Slug,
        ShortDescription = b.ShortDescription,
        Content          = b.Content,
        ThumbnailUrl     = b.ThumbnailUrl,
        Status           = b.Status.ToString(),
        RejectionReason  = b.RejectionReason,
        CategoryId       = b.CategoryId,
        CategoryName     = b.Category?.Name,
        AuthorId         = b.AuthorId,
        AuthorName       = GetAuthorName(b),
        CreatedAt        = b.CreatedAt,
        UpdatedAt        = b.UpdatedAt,
        PublishedAt      = b.PublishedAt
    };

    private static GetBlogByIdDto MapToGetBlogByIdDto(Blog b) => new()
    {
        Id               = b.Id,
        Title            = b.Title,
        Slug             = b.Slug,
        ShortDescription = b.ShortDescription,
        Content          = b.Content,
        ThumbnailUrl     = b.ThumbnailUrl,
        Status           = b.Status.ToString(),
        RejectionReason  = b.RejectionReason,
        CategoryId       = b.CategoryId,
        CategoryName     = b.Category?.Name,
        AuthorId         = b.AuthorId,
        AuthorName       = GetAuthorName(b),
        CreatedAt        = b.CreatedAt,
        UpdatedAt        = b.UpdatedAt,
        PublishedAt      = b.PublishedAt
    };

    private static CreateBlogDto MapToCreateBlogDto(Blog b) => new()
    {
        Id               = b.Id,
        Title            = b.Title,
        Slug             = b.Slug,
        ShortDescription = b.ShortDescription,
        Content          = b.Content,
        ThumbnailUrl     = b.ThumbnailUrl,
        Status           = (int)b.Status,
        CategoryId       = b.CategoryId,
        CategoryName     = b.Category?.Name,
        AuthorId         = b.AuthorId,
        AuthorName       = GetAuthorName(b),
        CreatedAt        = b.CreatedAt,
        UpdatedAt        = b.UpdatedAt,
        PublishedAt      = b.PublishedAt
    };

    private static UpdateBlogDto MapToUpdateBlogDto(Blog b) => new()
    {
        Id               = b.Id,
        Title            = b.Title,
        Slug             = b.Slug,
        ShortDescription = b.ShortDescription,
        Content          = b.Content,
        ThumbnailUrl     = b.ThumbnailUrl,
        Status           = b.Status.ToString(),
        RejectionReason  = b.RejectionReason,
        CategoryId       = b.CategoryId,
        CategoryName     = b.Category?.Name,
        AuthorId         = b.AuthorId,
        AuthorName       = GetAuthorName(b),
        CreatedAt        = b.CreatedAt,
        UpdatedAt        = b.UpdatedAt,
        PublishedAt      = b.PublishedAt
    };

    private static UpdateBlogStatusDto MapToUpdateBlogStatusDto(Blog b) => new()
    {
        Id              = b.Id,
        Title           = b.Title,
        Status          = b.Status.ToString(),
        RejectionReason = b.RejectionReason,
        PublishedAt     = b.PublishedAt,
        UpdatedAt       = b.UpdatedAt
    };
}