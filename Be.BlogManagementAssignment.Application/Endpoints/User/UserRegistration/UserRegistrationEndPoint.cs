using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;

public sealed class UserRegistrationEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/register",
            async (
                UserRegistrationRequest request,
                IUserService userService,
                IValidator<UserRegistrationRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                try
                {
                    var response = await userService.RegisterAsync(request);

                    if (!response.Status)
                    {
                        return Results.Conflict(response);
                    }

                    return Results.Ok(response);
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(new UserRegistrationResponse
                    {
                        Status  = false,
                        Message = ex.Message
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
            .WithName("userRegistration")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new Author or Visitor account in the Blog Management System. Role must be Author (2) or Visitor (3).")
            .Produces<UserRegistrationResponse>(200)
            .Produces<UserRegistrationResponse>(409)
            .ProducesValidationProblem()
            .ProducesProblem(500);
    }
}
