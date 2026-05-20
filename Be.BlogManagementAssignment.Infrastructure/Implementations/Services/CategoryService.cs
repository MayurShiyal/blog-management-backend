using Be.BlogManagementAssignment.Application.Endpoints.Category.ActiveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AllCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.CategoriesById;
using Be.BlogManagementAssignment.Application.Endpoints.Category.RemoveCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;
using Be.BlogManagementAssignment.Application.Exceptions;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<GetAllCategoriesResponse> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _categoryRepository.GetAllAsync(
            pageNumber, pageSize, search, cancellationToken);

        return new GetAllCategoriesResponse
        {
            Status = true,
            Message = "Categories retrieved successfully.",
            Items = items.Select(MapToGetAllCategoriesItemDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<GetCategoryByIdResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        return new GetCategoryByIdResponse
        {
            Status = true,
            Message = "Category retrieved successfully.",
            Data = MapToGetCategoryByIdDto(category)
        };
    }

    public async Task<GetActiveCategoriesResponse> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var (items, _) = await _categoryRepository.GetAllAsync(1, 1000, null, cancellationToken);
        var active = items.Where(c => c.IsActive).OrderBy(c => c.Name);

        return new GetActiveCategoriesResponse
        {
            Status = true,
            Message = "Active categories retrieved successfully.",
            Data = active.Select(MapToGetActiveCategoriesItemDto)
        };
    }

    public async Task<CreateCategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _categoryRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
            throw new ConflictException($"A category with the name '{name}' already exists.");

        if (await _categoryRepository.ExistsBySlugAsync(slug, cancellationToken: cancellationToken))
            throw new ConflictException($"A category with the slug '{slug}' already exists.");

        var category = new Category
        {
            Name = name,
            Slug = slug,
            Description = request.Description?.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        Category created = await _categoryRepository.CreateAsync(category, cancellationToken);

        _logger.LogInformation("Category created: {Name} (id={Id})", created.Name, created.Id);

        return new CreateCategoryResponse
        {
            Status = true,
            Message = "Category created successfully.",
            Data = MapToCreateCategoryDto(created)
        };
    }

    public async Task<UpdateCategoryResponse> UpdateAsync(
        int id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _categoryRepository.ExistsByNameAsync(name, excludeId: id, cancellationToken: cancellationToken))
            throw new ConflictException($"A category with the name '{name}' already exists.");

        if (await _categoryRepository.ExistsBySlugAsync(slug, excludeId: id, cancellationToken: cancellationToken))
            throw new ConflictException($"A category with the slug '{slug}' already exists.");

        existing.Name = name;
        existing.Slug = slug;
        existing.Description = request.Description?.Trim();
        existing.IsActive = request.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(existing, cancellationToken);

        _logger.LogInformation("Category updated: {Name} (id={Id})", existing.Name, existing.Id);

        return new UpdateCategoryResponse
        {
            Status = true,
            Message = "Category updated successfully.",
            Data = MapToUpdateCategoryDto(existing)
        };
    }

    public async Task<DeleteCategoryResponse> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category with id {id} was not found.");

        await _categoryRepository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("Category deleted: {Name} (id={Id})", existing.Name, id);

        return new DeleteCategoryResponse
        {
            Status = true,
            Message = "Category deleted successfully."
        };
    }

    private static GetAllCategoriesItemDto MapToGetAllCategoriesItemDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static GetCategoryByIdDto MapToGetCategoryByIdDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static GetActiveCategoriesItemDto MapToGetActiveCategoriesItemDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static CreateCategoryDto MapToCreateCategoryDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static UpdateCategoryDto MapToUpdateCategoryDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
