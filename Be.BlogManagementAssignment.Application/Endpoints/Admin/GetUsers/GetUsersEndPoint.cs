using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Admin.GetUsers;

public sealed class GetUsersEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/",
            async (
                IAdminUserService adminUserService,
                string? search = null,
                UserRole? role = null,
                UserStatus? status = null,
                int pageNumber = 1,
                int pageSize = 10,
                CancellationToken cancellationToken = default) =>
            {
                var result = await adminUserService.GetUsersAsync(
                    search, role, status, pageNumber, pageSize, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("getAdminUsers")
            .WithSummary("Get all users (Admin only)")
            .WithDescription(
                "Returns a paginated list of users. Supports search by name/email, " +
                "role filter (Author/Visitor), and status filter (Active/Inactive). " +
                "Soft-deleted users are excluded.")
            .Produces<GetUsersResponse>(200)
            .RequireAuthorization("AdminOnly");
    }
}
