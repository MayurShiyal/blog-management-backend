using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Api.Module.Blog;
using Be.BlogManagementAssignment.Api.Module.Category;
using Be.BlogManagementAssignment.Api.Module.User;

namespace Be.BlogManagementAssignment.Api.Extentions;

public static class MapModules
{
    public static void MapAllModules(this IEndpointRouteBuilder app)
    {
        var modules = new List<IModule>
        {
            new UserModule(),
            new CategoryModule(),
            new BlogModule(),
        };

        foreach (var module in modules)
        {
            module.AddRoutes(app);
        }
    }
}
