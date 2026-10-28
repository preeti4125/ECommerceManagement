using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CategoryRepository
{
    public async Task<int> AddAsync(Category category)
    {
        const string sql = """
            INSERT INTO Categories (CategoryName, Description, IsActive)
            VALUES (@CategoryName, @Description, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = (object?)category.Description ?? DBNull.Value;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = category.IsActive;

        await conn.OpenAsync();
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<List<Category>> GetAllAsync()
    {
        const string sql = "SELECT CategoryId, CategoryName, Description, IsActive FROM Categories ORDER BY CategoryId;";
        var list = new List<Category>();

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new Category
            {
                CategoryId = reader.GetInt32(0),
                CategoryName = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            });
        }
        return list;
    }
}