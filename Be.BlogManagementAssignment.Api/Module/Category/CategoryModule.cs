using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.Category.ActiveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AllCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.CategoriesById;
using Be.BlogManagementAssignment.Application.Endpoints.Category.RemoveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;

namespace Be.BlogManagementAssignment.Api.Module.Category;

public class CategoryModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/categories")
            .WithTags("Category");

        group.MapEndPoints<GetActiveCategoriesEndPoint>();
        group.MapEndPoints<GetAllCategoriesEndPoint>();
        group.MapEndPoints<GetCategoryByIdEndPoint>();
        group.MapEndPoints<CreateCategoryEndPoint>();
        group.MapEndPoints<UpdateCategoryEndPoint>();
        group.MapEndPoints<DeleteCategoryEndPoint>();
    }
}
