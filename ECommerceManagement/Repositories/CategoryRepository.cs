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

    public async Task<List<Category>> GetAllAsync(bool includeInactive = false)
    {
        string sql = "SELECT CategoryId, CategoryName, Description, IsActive FROM Categories";
        if (!includeInactive)
        {
            sql += " WHERE IsActive = 1";
        }
        sql += " ORDER BY CategoryId;";

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

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        const string sql = "SELECT CategoryId, CategoryName, Description, IsActive FROM Categories WHERE CategoryId = @CategoryId;";
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;

        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Category
            {
                CategoryId = reader.GetInt32(0),
                CategoryName = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            };
        }
        return null;
    }

    public async Task<bool> NameExistsAsync(string categoryName, int excludeCategoryId = 0)
    {
        const string sql = """
            SELECT COUNT(1) 
            FROM Categories 
            WHERE CategoryName = @CategoryName AND CategoryId <> @ExcludeId;
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = categoryName;
        cmd.Parameters.Add("@ExcludeId", SqlDbType.Int).Value = excludeCategoryId;

        await conn.OpenAsync();
        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        const string sql = """
            UPDATE Categories
            SET CategoryName = @CategoryName,
                Description = @Description,
                IsActive = @IsActive
            WHERE CategoryId = @CategoryId;
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = (object?)category.Description ?? DBNull.Value;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = category.IsActive;
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = category.CategoryId;

        await conn.OpenAsync();
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeactivateAsync(int categoryId)
    {
        const string sql = "UPDATE Categories SET IsActive = 0 WHERE CategoryId = @CategoryId;";
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;

        await conn.OpenAsync();
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }
}