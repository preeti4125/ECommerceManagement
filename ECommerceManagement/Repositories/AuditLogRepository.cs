using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class AuditLogRepository
{
    public async Task<List<AuditLog>> GetRecentLogsAsync(int count = 10)
    {
        const string sql = """
            SELECT TOP (@Count) AuditId, EntityName, EntityId, Action, Details, CreatedAt
            FROM AuditLogs
            ORDER BY CreatedAt DESC;
            """;

        var logs = new List<AuditLog>();
        await using SqlConnection conn = DatabaseHelper.CreateConnection();
        await using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@Count", count);

        await conn.OpenAsync();
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            logs.Add(new AuditLog
            {
                AuditId = reader.GetInt64(0),
                EntityName = reader.GetString(1),
                EntityId = reader.GetInt32(2),
                Action = reader.GetString(3),
                Details = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }

        return logs;
    }
}