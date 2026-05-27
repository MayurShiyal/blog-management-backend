using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.DeleteUser;

public sealed class DeleteUserEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/{id:guid}",
            async (Guid id, IAdminUserService adminUserService, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await adminUserService.DeleteUserAsync(id, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new DeleteUserResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex)
                {
                    return Results.BadRequest(new DeleteUserResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("deleteAdminUser")
            .WithSummary("Soft-delete a user (Admin only)")
            .WithDescription(
                "Marks the user as deleted (IsDeleted = true). " +
                "The record remains in the database but is excluded from all queries. " +
                "Admin accounts cannot be deleted.")
            .Produces<DeleteUserResponse>(200)
            .Produces<DeleteUserResponse>(400)
            .Produces<DeleteUserResponse>(404)
            .RequireAuthorization("AdminOnly");
    }
}
