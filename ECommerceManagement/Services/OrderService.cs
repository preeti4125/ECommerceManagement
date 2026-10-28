using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class OrderService
{
    private readonly OrderRepository _orderRepo;
    private readonly ProductRepository _productRepo;

    public OrderService(OrderRepository orderRepo, ProductRepository productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<int> ProcessOrderAsync(int customerId, int productId, int quantity)
    {
        if (customerId <= 0) throw new ArgumentException("Invalid Customer ID.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new InvalidOperationException($"Product #{productId} does not exist.");

        if (!product.IsActive)
            throw new InvalidOperationException($"Product '{product.ProductName}' is inactive.");

        if (product.StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient inventory: only {product.StockQuantity} unit(s) remaining.");

        return await _orderRepo.PlaceOrderTransactionalAsync(customerId, productId, quantity, product.Price);
    }
}