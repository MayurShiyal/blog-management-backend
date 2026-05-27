using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;

public sealed class GetBlogsEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/",
            async (
                HttpContext _httpContext,
                IBlogService _blogService,
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
                var role   = _httpContext.User.FindFirst("Role")?.Value;
                var userId = _httpContext.User.FindFirst("UserId")?.Value;

                var result = await _blogService.GetBlogsAsync(
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
