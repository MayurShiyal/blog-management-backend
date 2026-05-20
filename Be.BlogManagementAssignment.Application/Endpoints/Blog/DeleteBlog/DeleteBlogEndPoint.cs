using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;

/// <summary>
/// DELETE /api/blogs/{id}
///
/// Role-based behaviour (resolved from JWT — no separate routes):
///   • Admin  → can delete any blog regardless of authorship.
///   • Author → can only delete their own blog; returns 403 for others.
///   • Other  → 401 Unauthorized.
///
/// All role and ownership logic is in BlogService.DeleteBlogAsync.
/// </summary>
public sealed class DeleteBlogEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/{id:guid}",
            async (Guid id, HttpContext httpContext, IBlogService blogService, CancellationToken cancellationToken) =>
            {
                var role   = httpContext.User.FindFirst("Role")?.Value;
                var userId = httpContext.User.FindFirst("UserId")?.Value;

                try
                {
                    var result = await blogService.DeleteBlogAsync(id, role, userId, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new DeleteBlogResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex) when (ex.StatusCode == 403)
                {
                    return Results.Forbid();
                }
            })
            .WithName("deleteBlog")
            .WithSummary("Delete a blog (role-aware)")
            .WithDescription(
                "Admins can delete any blog. Authors can delete only their own blog. " +
                "Returns 403 if an Author attempts to delete another author's blog.")
            .Produces<DeleteBlogResponse>(200)
            .Produces<DeleteBlogResponse>(404)
            .Produces(401)
            .Produces(403)
            .RequireAuthorization("AdminOrAuthor");
    }
}
