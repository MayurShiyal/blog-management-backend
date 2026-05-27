using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

public sealed class UpdateBlogEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateBlogRequest request,
                HttpContext _httpContext,
                IBlogService _blogService,
                IValidator<UpdateBlogRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var validation = await validator.ValidateAsync(request, cancellationToken);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                var role   = _httpContext.User.FindFirst("Role")?.Value;
                var userId = _httpContext.User.FindFirst("UserId")?.Value;

                try
                {
                    var result = await _blogService.UpdateBlogAsync(id, request, role, userId, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new UpdateBlogResponse { Status = false, Message = ex.Message });
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(new UpdateBlogResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex) when (ex.StatusCode == 403)
                {
                    return Results.Forbid();
                }
                catch (AppException ex)
                {
                    return Results.BadRequest(new UpdateBlogResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("updateBlog")
            .WithSummary("Update blog content (Author or Admin)")
            .WithDescription(
                "Updates content fields of a blog. " +
                "To approve or reject a blog use PATCH /api/blogs/{id}/status.")
            .Produces<UpdateBlogResponse>(200)
            .ProducesValidationProblem()
            .Produces<UpdateBlogResponse>(400)
            .Produces<UpdateBlogResponse>(404)
            .Produces<UpdateBlogResponse>(409)
            .Produces(403)
            .RequireAuthorization("AdminOrAuthor");
    }
}
