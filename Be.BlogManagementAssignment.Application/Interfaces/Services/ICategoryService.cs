using Be.BlogManagementAssignment.Application.Endpoints.Category.ActiveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AllCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.CategoriesById;
using Be.BlogManagementAssignment.Application.Endpoints.Category.RemoveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;

namespace Be.BlogManagementAssignment.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<GetAllCategoriesResponse> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<GetCategoryByIdResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<GetActiveCategoriesResponse> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<CreateCategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateCategoryResponse> UpdateAsync(
        int id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteCategoryResponse> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
