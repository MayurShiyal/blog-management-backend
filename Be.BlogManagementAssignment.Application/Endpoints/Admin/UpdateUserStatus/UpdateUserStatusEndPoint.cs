using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUserStatus;
public sealed class UpdateUserStatusEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "/{id:guid}/status",
            async (
                Guid id,
                UpdateUserStatusRequest request,
                IAdminUserService adminUserService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await adminUserService.UpdateUserStatusAsync(id, request, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new UpdateUserStatusResponse { Status = false, Message = ex.Message });
                }
                catch (AppException ex)
                {
                    return Results.BadRequest(new UpdateUserStatusResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("updateAdminUserStatus")
            .WithSummary("Activate or deactivate a user (Admin only)")
            .WithDescription("Status=Active activates the user. Status=Inactive deactivates the user.")
            .Produces<UpdateUserStatusResponse>(200)
            .Produces<UpdateUserStatusResponse>(400)
            .Produces<UpdateUserStatusResponse>(404)
            .RequireAuthorization("AdminOnly");
    }
}
