using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.RemoveCategories;

public sealed class DeleteCategoryEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/{id:int}",
            async (int id, ICategoryService categoryService) =>
            {
                try
                {
                    var result = await categoryService.DeleteAsync(id);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new DeleteCategoryResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex)
                {
                    return Results.Conflict(new DeleteCategoryResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("deleteCategory")
            .WithSummary("Delete a category")
            .Produces<DeleteCategoryResponse>(200)
            .Produces<DeleteCategoryResponse>(404)
            .Produces<DeleteCategoryResponse>(409)
            .RequireAuthorization("AdminOnly");
    }
}
