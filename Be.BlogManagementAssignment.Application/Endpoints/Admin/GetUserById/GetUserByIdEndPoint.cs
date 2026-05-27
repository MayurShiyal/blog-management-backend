using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUserById;

public sealed class GetUserByIdEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/{id:guid}",
            async (Guid id, IAdminUserService adminUserService, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await adminUserService.GetUserByIdAsync(id, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new GetUserByIdResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("getAdminUserById")
            .WithSummary("Get a user by ID (Admin only)")
            .WithDescription("Returns full details of a single user. Soft-deleted users are treated as not found.")
            .Produces<GetUserByIdResponse>(200)
            .Produces<GetUserByIdResponse>(404)
            .RequireAuthorization("AdminOnly");
    }
}
