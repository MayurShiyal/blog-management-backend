using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;

public sealed class ForgotPasswordEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/forgot-password",
            async (
                ForgotPasswordRequest request,
                IUserService userService,
                IValidator<ForgotPasswordRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                try
                {
                    var response = await userService.ForgotPasswordAsync(request);
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Internal Server Error",
                        detail: ex.Message,
                        statusCode: 500);
                }
            })
            .WithName("forgotPassword")
            .WithSummary("Request a password reset")
            .WithDescription("Generates a reset token and sends a password-reset link to the user's email.")
            .Produces<ForgotPasswordResponse>(200)
            .ProducesValidationProblem()
            .ProducesProblem(500);
    }
}
