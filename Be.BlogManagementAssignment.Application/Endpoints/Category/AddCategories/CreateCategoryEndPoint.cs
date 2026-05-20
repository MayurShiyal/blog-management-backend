using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;

public sealed class CreateCategoryEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/",
            async (
                CreateCategoryRequest request,
                ICategoryService categoryService,
                IValidator<CreateCategoryRequest> validator) =>
            {
                var validation = await validator.ValidateAsync(request);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                try
                {
                    var result = await categoryService.CreateAsync(request);
                    return Results.Created($"/api/categories/{result.Data?.Id}", result);
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(new CreateCategoryResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("createCategory")
            .WithSummary("Create a new category")
            .Produces<CreateCategoryResponse>(201)
            .ProducesValidationProblem()
            .Produces<CreateCategoryResponse>(409)
            .RequireAuthorization("AdminOnly");
    }
}
