using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.VerifyEmail;

/// <summary>
/// GET /api/user/verify-email?token={token}
///
/// Called when the user clicks the verification link in their email.
/// Marks IsVerified = true, stores the JWT in VerificationToken, and
/// returns a confirmation message. The JWT can then be retrieved on Login.
/// </summary>
public sealed class VerifyEmailEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/verify-email",
            async (string token, IUserService userService) =>
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("TOKEN RECEIVED = " + token);
                    return Results.BadRequest(new
                    {
                        Status = false,
                        Message = "Verification token is required."
                    });
                }

                try
                {
                    var result = await userService.VerifyEmailAsync(token);

                    if (!result)
                    {
                        return Results.BadRequest(new
                        {
                            Status = false,
                            Message = "Invalid or already-used verification token."
                        });
                    }

                    return Results.Ok(new
                    {
                        Status = true,
                        Message = "Email verified successfully. You can now log in to your account."
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Internal Server Error",
                        detail: ex.Message,
                        statusCode: 500);
                }
            })
            .WithName("verifyEmail")
            .WithSummary("Verify user email")
            .WithDescription("Verifies the user's email address using the one-time token sent via email. Sets IsVerified = true and stores the JWT token for the account.")
            .Produces(200)
            .Produces(400)
            .ProducesProblem(500);
    }
}
