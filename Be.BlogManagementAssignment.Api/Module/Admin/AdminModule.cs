using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.DeleteUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUserById;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;
using Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;

namespace Be.BlogManagementAssignment.Api.Module.Admin;
public class AdminModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/admin/users")
            .WithTags("Admin - User Management");

        group.MapEndPoints<GetUsersEndPoint>();
        group.MapEndPoints<GetUserByIdEndPoint>();
        group.MapEndPoints<UpdateUserEndPoint>();
        group.MapEndPoints<UpdateUserStatusEndPoint>();
        group.MapEndPoints<DeleteUserEndPoint>();
    }
}
