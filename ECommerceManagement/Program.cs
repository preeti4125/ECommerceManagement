using ECommerceManagement.Models;
using ECommerceManagement.Repositories;
using ECommerceManagement.Services;

var categoryRepo = new CategoryRepository();
var productRepo = new ProductRepository();
var customerRepo = new CustomerRepository();
var orderRepo = new OrderRepository();
var reportRepo = new ReportRepository();
var auditRepo = new AuditLogRepository();

var categoryService = new CategoryService(categoryRepo);
var productService = new ProductService(productRepo);
var customerService = new CustomerService(customerRepo);
var orderService = new OrderService(orderRepo, productRepo);

while (true)
{
    Console.Clear();
    Console.WriteLine("""
    ========================================
          ECOMMERCE MANAGEMENT SYSTEM       
    ========================================
    1.  Categories: List All
    2.  Categories: Add New
    3.  Categories: Deactivate (Soft Delete)
    4.  Products: List All
    5.  Products: Add New
    6.  Products: Search by Keyword
    7.  Products: Low Stock Alerts (< 10)
    8.  Customers: List All
    9.  Customers: Register New
    10. Orders: Place Order (Transaction Demo)
    11. Reports: Product Sales & Revenue
    12. Reports: Inactive Customers (0 Orders)
    13. Audit: View Recent Logs
    0.  Exit
    ========================================
    """);
    Console.Write("Enter your choice: ");
    string? input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1":
                var categories = await categoryService.GetAllCategoriesAsync(includeInactive: true);
                Console.WriteLine($"\n{"ID",-5} | {"Category Name",-25} | {"Status",-10} | Description");
                Console.WriteLine(new string('-', 70));
                foreach (var cat in categories)
                {
                    string status = cat.IsActive ? "Active" : "Inactive";
                    Console.WriteLine($"{cat.CategoryId,-5} | {cat.CategoryName,-25} | {status,-10} | {cat.Description}");
                }
                break;

            case "2":
                Console.Write("\nCategory Name: ");
                string cName = Console.ReadLine() ?? "";
                Console.Write("Description (optional): ");
                string? desc = Console.ReadLine();

                int newCatId = await categoryService.CreateCategoryAsync(new Category
                {
                    CategoryName = cName,
                    Description = string.IsNullOrWhiteSpace(desc) ? null : desc,
                    IsActive = true
                });
                Console.WriteLine($"\nSuccess! Category created with ID: {newCatId}");
                break;

            case "3":
                Console.Write("\nEnter Category ID to deactivate: ");
                int deactivateCatId = int.Parse(Console.ReadLine() ?? "0");
                await categoryService.DeactivateCategoryAsync(deactivateCatId);
                Console.WriteLine($"\nCategory #{deactivateCatId} deactivated successfully.");
                break;

            case "4":
                var products = await productService.GetAllAsync();
                Console.WriteLine($"\n{"ID",-5} | {"Product",-32} | {"Category",-15} | {"Price",-8} | {"Stock",-6}");
                Console.WriteLine(new string('-', 72));
                foreach (var p in products)
                {
                    Console.WriteLine($"{p.ProductId,-5} | {p.ProductName,-32} | {p.CategoryName,-15} | ${p.Price,-7:F2} | {p.StockQuantity,-6}");
                }
                break;

            case "5":
                Console.Write("\nProduct Name: ");
                string prodName = Console.ReadLine() ?? "";
                Console.Write("Price: ");
                decimal price = decimal.Parse(Console.ReadLine() ?? "0");
                Console.Write("Initial Stock: ");
                int stock = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Category ID: ");
                int catId = int.Parse(Console.ReadLine() ?? "1");

                int newProdId = await productService.CreateProductAsync(new Product
                {
                    ProductName = prodName,
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = catId,
                    IsActive = true
                });
                Console.WriteLine($"\nSuccess! Product added with ID: {newProdId}");
                break;

            case "6":
                Console.Write("\nEnter search keyword: ");
                string keyword = Console.ReadLine() ?? "";
                var matched = await productService.SearchProductsAsync(keyword);

                Console.WriteLine($"\n{"ID",-5} | {"Product",-32} | {"Category",-15} | {"Price",-8} | {"Stock",-6}");
                Console.WriteLine(new string('-', 72));
                foreach (var p in matched)
                {
                    Console.WriteLine($"{p.ProductId,-5} | {p.ProductName,-32} | {p.CategoryName,-15} | ${p.Price,-7:F2} | {p.StockQuantity,-6}");
                }
                break;

            case "7":
                var lowStock = await productService.GetLowStockAlertsAsync(10);
                Console.WriteLine("\n--- LOW STOCK ALERTS (Threshold < 10) ---");
                Console.WriteLine($"{"ID",-5} | {"Product",-32} | {"Category",-15} | {"Stock Remaining",-15}");
                Console.WriteLine(new string('-', 72));

                if (lowStock.Count == 0)
                {
                    Console.WriteLine("All products have adequate stock levels.");
                }
                else
                {
                    foreach (var p in lowStock)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{p.ProductId,-5} | {p.ProductName,-32} | {p.CategoryName,-15} | {p.StockQuantity,-15}");
                        Console.ResetColor();
                    }
                }
                break;

            case "8":
                var customers = await customerService.GetAllCustomersAsync();
                Console.WriteLine($"\n{"ID",-5} | {"Full Name",-20} | {"Email",-28} | {"Phone",-15}");
                Console.WriteLine(new string('-', 75));
                foreach (var c in customers)
                {
                    Console.WriteLine($"{c.CustomerId,-5} | {c.FullName,-20} | {c.Email,-28} | {c.PhoneNumber,-15}");
                }
                break;

            case "9":
                Console.Write("\nFull Name: ");
                string custName = Console.ReadLine() ?? "";
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Phone: ");
                string phone = Console.ReadLine() ?? "";
                Console.Write("Address (optional): ");
                string? address = Console.ReadLine();

                int newCustId = await customerService.RegisterCustomerAsync(new Customer
                {
                    FullName = custName,
                    Email = email,
                    PhoneNumber = phone,
                    Address = string.IsNullOrWhiteSpace(address) ? null : address,
                    IsActive = true
                });
                Console.WriteLine($"\nSuccess! Customer registered with ID: {newCustId}");
                break;

            case "10":
                Console.Write("\nCustomer ID: ");
                int cId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Product ID: ");
                int pId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Quantity: ");
                int qty = int.Parse(Console.ReadLine() ?? "0");

                int orderId = await orderService.ProcessOrderAsync(cId, pId, qty);
                Console.WriteLine($"\nSuccess! Order #{orderId} processed atomically with stock deduction and audit record.");
                break;

            case "11":
                await reportRepo.PrintProductSalesReportAsync();
                break;

            case "12":
                await reportRepo.PrintInactiveCustomersReportAsync();
                break;

            case "13":
                var logs = await auditRepo.GetRecentLogsAsync(10);
                Console.WriteLine($"\n{"ID",-5} | {"Entity",-8} | {"Action",-8} | {"Time",-20} | Details");
                Console.WriteLine(new string('-', 80));
                foreach (var log in logs)
                {
                    Console.WriteLine($"{log.AuditId,-5} | {log.EntityName,-8} | {log.Action,-8} | {log.CreatedAt:yyyy-MM-dd HH:mm:ss} | {log.Details}");
                }
                break;

            case "0":
                return;

            default:
                Console.WriteLine("\nInvalid option. Try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine("\nPress Enter to return to menu...");
    Console.ReadLine();
}