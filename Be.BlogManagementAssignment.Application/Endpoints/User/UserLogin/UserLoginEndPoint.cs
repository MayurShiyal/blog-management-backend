using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;

public sealed class UserLoginEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/login",
            async (
                UserLoginRequest request,
                IUserService userService,
                IValidator<UserLoginRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                try
                {
                    var response = await userService.LoginAsync(request);

                    if (!response.Status)
                    {
                        return Results.Json(
                            response,
                            statusCode: StatusCodes.Status401Unauthorized);
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
            .WithName("userLogin")
            .WithSummary("Login a user")
            .WithDescription("Authenticates an Author or Visitor and returns a JWT bearer token. A verification email is sent on each successful login.")
            .Produces<UserLoginResponse>(200)
            .Produces<UserLoginResponse>(401)
            .ProducesValidationProblem()
            .ProducesProblem(500);
    }
}
