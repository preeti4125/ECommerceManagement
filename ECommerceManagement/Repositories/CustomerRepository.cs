using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CustomerRepository
{
    public async Task<int> AddAsync(Customer customer)
    {
        const string sql = """
            INSERT INTO Customers (FullName, Email, PhoneNumber, Address, IsActive)
            VALUES (@FullName, @Email, @PhoneNumber, @Address, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = customer.FullName;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = customer.Email;
        cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = customer.PhoneNumber;
        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = (object?)customer.Address ?? DBNull.Value;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = customer.IsActive;

        await conn.OpenAsync();
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        const string sql = "SELECT CustomerId, FullName, Email, PhoneNumber, Address, CreatedAt, IsActive FROM Customers ORDER BY CustomerId;";
        var list = new List<Customer>();

        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        await conn.OpenAsync();

        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
                PhoneNumber = reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                IsActive = reader.GetBoolean(6)
            });
        }
        return list;
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        const string sql = "SELECT CustomerId, FullName, Email, PhoneNumber, Address, CreatedAt, IsActive FROM Customers WHERE CustomerId = @CustomerId;";
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;

        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
                PhoneNumber = reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                IsActive = reader.GetBoolean(6)
            };
        }
        return null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM Customers WHERE Email = @Email;";
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;

        await conn.OpenAsync();
        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }
}