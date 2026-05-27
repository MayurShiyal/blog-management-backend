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
        Guid? authorId,
        CancellationToken cancellationToken = default);

    Task<GetBlogByIdResponse> GetBlogByIdAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<CreateBlogResponse> CreateAsync(
        CreateBlogRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default);

    Task<UpdateBlogResponse> UpdateBlogAsync(
        Guid blogId,
        UpdateBlogRequest request,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<UpdateBlogStatusResponse> UpdateBlogStatusAsync(
        Guid blogId,
        UpdateBlogStatusRequest request,
        string? role,
        CancellationToken cancellationToken = default);

    Task<DeleteBlogResponse> DeleteBlogAsync(
        Guid blogId,
        string? role,
        string? userId,
        CancellationToken cancellationToken = default);
}
