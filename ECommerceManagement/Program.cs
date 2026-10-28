using ECommerceManagement.Models;
using ECommerceManagement.Repositories;
using ECommerceManagement.Services;

var productRepo = new ProductRepository();
var orderRepo = new OrderRepository();

var productService = new ProductService(productRepo);
var orderService = new OrderService(orderRepo, productRepo);

while (true)
{
    Console.Clear();
    Console.WriteLine("""
    ========================================
          ECOMMERCE MANAGEMENT SYSTEM       
    ========================================
    1. List All Products
    2. Add New Product
    3. Place Order (Transaction Demo)
    0. Exit
    ========================================
    """);
    Console.Write("Enter your choice: ");
    string? input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1":
                var products = await productService.GetAllAsync();
                Console.WriteLine($"\n{"ID",-5} | {"Product",-32} | {"Category",-15} | {"Price",-8} | {"Stock",-6}");
                Console.WriteLine(new string('-', 72));
                foreach (var p in products)
                {
                    Console.WriteLine($"{p.ProductId,-5} | {p.ProductName,-32} | {p.CategoryName,-15} | ${p.Price,-7:F2} | {p.StockQuantity,-6}");
                }
                break;

            case "2":
                Console.Write("\nProduct Name: ");
                string name = Console.ReadLine() ?? "";
                Console.Write("Price: ");
                decimal price = decimal.Parse(Console.ReadLine() ?? "0");
                Console.Write("Initial Stock: ");
                int stock = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Category ID (1 = Electronics, 2 = Apparel, 3 = Home): ");
                int catId = int.Parse(Console.ReadLine() ?? "1");

                int newId = await productService.CreateProductAsync(new Product
                {
                    ProductName = name,
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = catId,
                    IsActive = true
                });
                Console.WriteLine($"\nSuccess! Product added with ID: {newId}");
                break;

            case "3":
                Console.Write("\nCustomer ID (e.g. 1 or 2): ");
                int custId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Product ID: ");
                int prodId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Quantity: ");
                int qty = int.Parse(Console.ReadLine() ?? "0");

                int orderId = await orderService.ProcessOrderAsync(custId, prodId, qty);
                Console.WriteLine($"\nSuccess! Order #{orderId} created, stock deducted, and audit trail saved atomically.");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("\nInvalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine("\nPress Enter to return to the menu...");
    Console.ReadLine();
}