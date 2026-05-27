using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;

public sealed class ResetPasswordEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/reset-password",
            async (
                ResetPasswordRequest request,
                IUserService _userService,
                IValidator<ResetPasswordRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                try
                {
                    var response = await _userService.ResetPasswordAsync(request);

                    if (!response.Status)
                    {
                        return Results.Json(
                            response,
                            statusCode: StatusCodes.Status400BadRequest);
                    }

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
            .WithName("resetPassword")
            .WithSummary("Reset user password")
            .WithDescription("Validates the reset token and updates the user's password.")
            .Produces<ResetPasswordResponse>(200)
            .Produces<ResetPasswordResponse>(400)
            .ProducesValidationProblem()
            .ProducesProblem(500);
    }
}
