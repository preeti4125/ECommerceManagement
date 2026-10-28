using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class ProductRepository
{
    public async Task<int> AddAsync(Product product)
    {
        const string sql = """
            INSERT INTO Products (ProductName, Price, StockQuantity, CategoryId, IsActive)
            VALUES (@ProductName, @Price, @StockQuantity, @CategoryId, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = product.ProductName;
        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;
        cmd.Parameters.Add("@StockQuantity", SqlDbType.Int).Value = product.StockQuantity;
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = product.CategoryId;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = product.IsActive;

        await conn.OpenAsync();
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<List<Product>> GetAllWithCategoryAsync()
    {
        const string sql = """
            SELECT p.ProductId, p.ProductName, p.Price, p.StockQuantity, p.CategoryId, p.IsActive, c.CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.CategoryId
            ORDER BY p.ProductId;
            """;

        var list = new List<Product>();
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        await conn.OpenAsync();

        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Price = reader.GetDecimal(2),
                StockQuantity = reader.GetInt32(3),
                CategoryId = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5),
                CategoryName = reader.GetString(6)
            });
        }
        return list;
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        const string sql = "SELECT ProductId, ProductName, Price, StockQuantity, CategoryId, IsActive FROM Products WHERE ProductId = @ProductId;";
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Price = reader.GetDecimal(2),
                StockQuantity = reader.GetInt32(3),
                CategoryId = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }
}