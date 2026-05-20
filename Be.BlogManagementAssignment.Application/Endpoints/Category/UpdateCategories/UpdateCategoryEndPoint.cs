using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Extentions;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;

public sealed class UpdateCategoryEndPoint : IMinimalEndPoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/{id:int}",
            async (
                int id,
                UpdateCategoryRequest request,
                ICategoryService categoryService,
                IValidator<UpdateCategoryRequest> validator) =>
            {
                var validation = await validator.ValidateAsync(request);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                try
                {
                    var result = await categoryService.UpdateAsync(id, request);
                    return Results.Ok(result);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(new UpdateCategoryResponse { Status = false, Message = ex.Message });
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(new UpdateCategoryResponse { Status = false, Message = ex.Message });
                }
            })
            .WithName("updateCategory")
            .WithSummary("Update an existing category")
            .Produces<UpdateCategoryResponse>(200)
            .ProducesValidationProblem()
            .Produces<UpdateCategoryResponse>(404)
            .Produces<UpdateCategoryResponse>(409)
            .RequireAuthorization("AdminOnly");
    }
}
