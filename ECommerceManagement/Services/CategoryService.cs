using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;

    public CategoryService(CategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> CreateCategoryAsync(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category name cannot be empty.");

        if (await _repository.NameExistsAsync(category.CategoryName.Trim()))
            throw new InvalidOperationException($"Category '{category.CategoryName}' already exists.");

        category.CategoryName = category.CategoryName.Trim();
        return await _repository.AddAsync(category);
    }

    public Task<List<Category>> GetAllCategoriesAsync(bool includeInactive = false)
        => _repository.GetAllAsync(includeInactive);

    public async Task DeactivateCategoryAsync(int categoryId)
    {
        var existing = await _repository.GetByIdAsync(categoryId)
            ?? throw new KeyNotFoundException($"Category #{categoryId} was not found.");

        if (!existing.IsActive)
            throw new InvalidOperationException($"Category #{categoryId} is already inactive.");

        await _repository.DeactivateAsync(categoryId);
    }
}