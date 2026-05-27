using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public sealed class CreateBlogEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/",
            async (
                CreateBlogRequest request,
                HttpContext _httpContext,
                IBlogService _blogService,
                IValidator<CreateBlogRequest> validator) =>
            {
                var validation = await validator.ValidateAsync(request);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                var authorId = GetAuthorId(_httpContext);
                if (authorId == null)
                    return Results.Unauthorized();

                try
                {
                    var result = await _blogService.CreateAsync(request, authorId.Value);
                    return Results.Created($"/api/blogs/{result.Data?.Id}", result);
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(new CreateBlogResponse { Status = false, Message = ex.Message });
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new CreateBlogResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex)
                {
                    return Results.BadRequest(new CreateBlogResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("createBlog")
            .WithSummary("Create a new blog (Author)")
            .Produces<CreateBlogResponse>(201)
            .ProducesValidationProblem()
            .Produces<CreateBlogResponse>(400)
            .Produces<CreateBlogResponse>(404)
            .Produces<CreateBlogResponse>(409)
            .RequireAuthorization("AuthorOnly");
    }

    private static Guid? GetAuthorId(HttpContext _httpContext)
    {
        var userIdClaim = _httpContext.User.FindFirst("UserId")?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : null;
    }
}
