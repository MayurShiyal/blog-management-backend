using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.ActiveCategories;

public sealed class GetActiveCategoriesEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/active",
            async (ICategoryService categoryService) =>
            {
                var result = await categoryService.GetActiveAsync();
                return Results.Ok(result);
            })
            .WithName("getActiveCategories")
            .WithSummary("Get active categories")
            .WithDescription("Returns all active categories. Used by Authors when creating blog posts.")
            .Produces<GetActiveCategoriesResponse>(200)
            .RequireAuthorization();
    }
}
