using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class ProductService
{
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository) => _repository = repository;

    public async Task<int> CreateProductAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductName))
            throw new ArgumentException("Product name cannot be empty.");

        if (product.Price <= 0)
            throw new ArgumentOutOfRangeException(nameof(product.Price), "Price must be strictly greater than 0.");

        if (product.StockQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(product.StockQuantity), "Initial stock cannot be negative.");

        if (product.CategoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(product.CategoryId), "Select a valid category ID.");

        return await _repository.AddAsync(product);
    }

    public Task<List<Product>> GetAllAsync() => _repository.GetAllWithCategoryAsync();
    public Task<Product?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<List<Product>> SearchProductsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return _repository.GetAllWithCategoryAsync();

        return _repository.SearchByNameAsync(keyword.Trim());
    }

    public Task<List<Product>> GetLowStockAlertsAsync(int threshold = 10)
    {
        return _repository.GetLowStockProductsAsync(threshold);
    }
}