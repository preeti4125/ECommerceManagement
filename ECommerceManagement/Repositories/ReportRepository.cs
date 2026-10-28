using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;

namespace ECommerceManagement.Repositories;

public class ReportRepository
{
    // Report 1: Sales volume and revenue per product
    public async Task PrintProductSalesReportAsync()
    {
        const string sql = """
            SELECT 
                p.ProductId, 
                p.ProductName, 
                COALESCE(SUM(oi.Quantity), 0) AS TotalUnitsSold,
                COALESCE(SUM(oi.Quantity * oi.UnitPrice), 0.00) AS TotalRevenue
            FROM Products p
            LEFT JOIN OrderItems oi ON oi.ProductId = p.ProductId
            GROUP BY p.ProductId, p.ProductName
            ORDER BY TotalRevenue DESC;
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        Console.WriteLine("\n--- PRODUCT SALES REPORT ---");
        Console.WriteLine($"{"ID",-5} | {"Product Name",-35} | {"Sold",-6} | {"Revenue",-12}");
        Console.WriteLine(new string('-', 66));

        while (await reader.ReadAsync())
        {
            Console.WriteLine($"{reader.GetInt32(0),-5} | {reader.GetString(1),-35} | {reader.GetInt32(2),-6} | ${reader.GetDecimal(3),-10:F2}");
        }
    }

    // Report 2: Customers who have never placed an order
    public async Task PrintInactiveCustomersReportAsync()
    {
        const string sql = """
            SELECT c.CustomerId, c.FullName, c.Email
            FROM Customers c
            WHERE NOT EXISTS (
                SELECT 1 FROM Orders o
                WHERE o.CustomerId = c.CustomerId
            );
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        Console.WriteLine("\n--- CUSTOMERS WITH ZERO ORDERS ---");
        Console.WriteLine($"{"ID",-5} | {"Customer Name",-25} | {"Email",-30}");
        Console.WriteLine(new string('-', 66));

        bool hasRows = false;
        while (await reader.ReadAsync())
        {
            hasRows = true;
            Console.WriteLine($"{reader.GetInt32(0),-5} | {reader.GetString(1),-25} | {reader.GetString(2),-30}");
        }

        if (!hasRows)
        {
            Console.WriteLine("None! All registered customers have placed at least one order.");
        }
    }
}