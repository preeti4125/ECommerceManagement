namespace ECommerceManagement.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? CategoryName { get; set; }
}