using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

public sealed class UpdateBlogStatusEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "/{id:guid}/status",
            async (
                Guid id,
                UpdateBlogStatusRequest request,
                HttpContext _httpContext,
                IBlogService _blogService,
                CancellationToken cancellationToken) =>
            {
                var role = _httpContext.User.FindFirst("Role")?.Value;

                try
                {
                    var result = await _blogService.UpdateBlogStatusAsync(id, request, role, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new UpdateBlogStatusResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex) when (ex.StatusCode == 403)
                {
                    return Results.Forbid();
                }
                catch (AppException ex)
                {
                    return Results.BadRequest(new UpdateBlogStatusResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("updateBlogStatus")
            .WithSummary("Approve or reject a blog (Admin only)")
            .WithDescription(
                "Status=Published approves a PendingApproval blog. " +
                "Status=Rejected rejects it (RejectionReason required).")
            .Produces<UpdateBlogStatusResponse>(200)
            .Produces<UpdateBlogStatusResponse>(400)
            .Produces<UpdateBlogStatusResponse>(404)
            .Produces(403)
            .RequireAuthorization("AdminOnly");
    }
}
