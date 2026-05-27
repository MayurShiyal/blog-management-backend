using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;

public sealed class GetBlogByIdEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/{id:guid}",
            async (Guid id, HttpContext _httpContext, IBlogService _blogService, CancellationToken cancellationToken) =>
            {
                var role   = _httpContext.User.FindFirst("Role")?.Value;
                var userId = _httpContext.User.FindFirst("UserId")?.Value;

                try
                {
                    var result = await _blogService.GetBlogByIdAsync(id, role, userId, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new GetBlogByIdResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex) when (ex.StatusCode == 403)
                {
                    return Results.Forbid();
                }
            })
            .WithName("getBlogById")
            .WithSummary("Get blog by ID (role-aware)")
            .WithDescription(
                "Admins can retrieve any blog. Authors can retrieve their own blog only. " +
                "Unauthenticated users can only retrieve published blogs.")
            .Produces<GetBlogByIdResponse>(200)
            .Produces<GetBlogByIdResponse>(404)
            .Produces(403)
            .AllowAnonymous();
    }
}
