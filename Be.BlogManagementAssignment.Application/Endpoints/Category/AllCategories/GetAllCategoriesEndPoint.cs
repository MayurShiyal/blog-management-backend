using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.AllCategories;

public sealed class GetAllCategoriesEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/",
            async (
                ICategoryService categoryService,
                int pageNumber = 1,
                int pageSize = 10,
                string? search = null) =>
            {
                var result = await categoryService.GetAllAsync(pageNumber, pageSize, search);
                return Results.Ok(result);
            })
            .WithName("getAllCategories")
            .WithSummary("Get all categories (paginated)")
            .WithDescription("Returns a paginated list of categories. Supports optional search by name.")
            .Produces<GetAllCategoriesResponse>(200)
            .RequireAuthorization();
    }
}
