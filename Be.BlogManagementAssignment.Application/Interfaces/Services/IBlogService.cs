using Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;
using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface IBlogService
{
    // ── GET /api/blogs ───────────────────────────────────────────────────
    Task<GetBlogsResponse> GetBlogsAsync(
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
        CancellationToken cancellationToken = default);

    // ── GET /api/blogs/{id} ──────────────────────────────────────────────
    Task<GetBlogByIdResponse> GetBlogByIdAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);

    // ── POST /api/blogs ──────────────────────────────────────────────────
    Task<CreateBlogResponse> CreateAsync(
        CreateBlogRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default);

    // ── PUT /api/blogs/{id} ──────────────────────────────────────────────
    /// <summary>
    /// Updates blog content fields only (title, slug, description, content,
    /// thumbnail, category). Does not change blog status.
    /// </summary>
    Task<UpdateBlogResponse> UpdateBlogAsync(
        Guid blogId,
        UpdateBlogRequest request,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);

    // ── PATCH /api/blogs/{id}/status ─────────────────────────────────────
    /// <summary>
    /// Admin-only: approve (Published) or reject (Rejected) a PendingApproval blog.
    /// </summary>
    Task<UpdateBlogStatusResponse> UpdateBlogStatusAsync(
        Guid blogId,
        UpdateBlogStatusRequest request,
        string? role,
        CancellationToken cancellationToken = default);

    // ── DELETE /api/blogs/{id} ───────────────────────────────────────────
    Task<DeleteBlogResponse> DeleteBlogAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);
}
