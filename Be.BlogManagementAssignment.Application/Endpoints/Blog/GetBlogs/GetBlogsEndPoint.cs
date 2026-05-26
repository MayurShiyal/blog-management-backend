using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;

/// <summary>
/// GET /api/blogs
///
/// Single unified endpoint for all blog list retrieval.
/// Role-based filtering, slug lookup, status filter, search, pagination,
/// and sort are all handled via query parameters.
///
/// Role behaviour (resolved from JWT — no separate routes):
///   • Admin   → all blogs, all authors, all statuses
///   • Author  → own blogs only (mine=true implied, overridable by mine=false for public feed)
///   • Public  → Published blogs only
///
/// Query parameters (all optional):
///   slug, status, search, mine, pageNumber, pageSize, sortBy, sortDesc, categoryId
/// </summary>
public sealed class GetBlogsEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/",
            async (
                HttpContext httpContext,
                IBlogService blogService,
                string? slug = null,
                BlogStatus? status = null,
                string? search = null,
                bool? mine = null,
                int pageNumber = 1,
                int pageSize = 10,
                string? sortBy = null,
                bool sortDesc = true,
                int? categoryId = null,
                Guid? authorId = null,
                CancellationToken cancellationToken = default) =>
            {
                var role   = httpContext.User.FindFirst("Role")?.Value;
                var userId = httpContext.User.FindFirst("UserId")?.Value;

                var result = await blogService.GetBlogsAsync(
                    role, userId, slug, status, search, mine,
                    pageNumber, pageSize, sortBy, sortDesc, categoryId,authorId,
                    cancellationToken);

                return Results.Ok(result);
            })
            .WithName("getBlogs")
            .WithSummary("Get blogs (role-aware, unified)")
            .WithDescription(
                "Returns blogs filtered by the caller's role and query parameters. " +
                "Supports slug lookup, status filter, free-text search, mine flag, " +
                "pagination, and sorting. " +
                "Admins receive all blogs; Authors receive their own; anonymous users receive only published blogs.")
            .Produces<GetBlogsResponse>(200)
            .AllowAnonymous();
    }
}
