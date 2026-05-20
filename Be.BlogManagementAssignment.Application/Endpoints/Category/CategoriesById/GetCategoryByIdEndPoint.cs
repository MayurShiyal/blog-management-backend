using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.CategoriesById;

public sealed class GetCategoryByIdEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/{id:int}",
            async (int id, ICategoryService categoryService) =>
            {
                try
                {
                    var result = await categoryService.GetByIdAsync(id);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new GetCategoryByIdResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("getCategoryById")
            .WithSummary("Get category by id")
            .Produces<GetCategoryByIdResponse>(200)
            .Produces<GetCategoryByIdResponse>(404)
            .RequireAuthorization();
    }
}
