using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.UpdateUser;

public sealed class UpdateUserEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateUserRequest request,
                IAdminUserService adminUserService,
                IValidator<UpdateUserRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var validation = await validator.ValidateAsync(request, cancellationToken);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage);
                    return Results.BadRequest(new UpdateUserResponse
                    {
                        Status = false,
                        Message = string.Join(" ", errors)
                    });
                }

                try
                {
                    var result = await adminUserService.UpdateUserAsync(id, request, cancellationToken);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new UpdateUserResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("updateAdminUser")
            .WithSummary("Update a user's name (Admin only)")
            .WithDescription("Updates a user's first name and last name. Only non-deleted users can be updated.")
            .Produces<UpdateUserResponse>(200)
            .Produces<UpdateUserResponse>(400)
            .Produces<UpdateUserResponse>(404)
            .RequireAuthorization("AdminOnly");
    }
}
