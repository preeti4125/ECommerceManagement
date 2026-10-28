using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;

namespace ECommerceManagement.Repositories;

public class OrderRepository
{
    public async Task<int> PlaceOrderTransactionalAsync(int customerId, int productId, int quantity, decimal unitPrice)
    {
        await using SqlConnection connection = DatabaseHelper.CreateConnection();
        await connection.OpenAsync();

        await using SqlTransaction transaction =
            (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            decimal totalAmount = unitPrice * quantity;

            // 1. Insert Order
            const string orderSql = """
                INSERT INTO Orders (CustomerId, TotalAmount, Status)
                VALUES (@CustomerId, @TotalAmount, N'Completed');
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            int orderId;
            await using (SqlCommand orderCmd = new(orderSql, connection, transaction))
            {
                orderCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
                orderCmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = totalAmount;
                orderId = Convert.ToInt32(await orderCmd.ExecuteScalarAsync());
            }

            // 2. Insert Order Item
            const string itemSql = """
                INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
                """;

            await using (SqlCommand itemCmd = new(itemSql, connection, transaction))
            {
                itemCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                itemCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                itemCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;
                itemCmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = unitPrice;
                await itemCmd.ExecuteNonQueryAsync();
            }

            // 3. Deduct Stock
            const string stockSql = """
                UPDATE Products
                SET StockQuantity = StockQuantity - @Quantity
                WHERE ProductId = @ProductId AND StockQuantity >= @Quantity;
                """;

            await using (SqlCommand stockCmd = new(stockSql, connection, transaction))
            {
                stockCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;
                stockCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

                int affected = await stockCmd.ExecuteNonQueryAsync();
                if (affected == 0)
                {
                    throw new InvalidOperationException("Transaction aborted: Insufficient stock available.");
                }
            }

            // 4. Insert Audit Log
            const string auditSql = """
                INSERT INTO AuditLogs (EntityName, EntityId, Action, Details)
                VALUES (@EntityName, @EntityId, @Action, @Details);
                """;

            await using (SqlCommand auditCmd = new(auditSql, connection, transaction))
            {
                auditCmd.Parameters.Add("@EntityName", SqlDbType.NVarChar, 50).Value = "Orders";
                auditCmd.Parameters.Add("@EntityId", SqlDbType.Int).Value = orderId;
                auditCmd.Parameters.Add("@Action", SqlDbType.NVarChar, 20).Value = "INSERT";
                auditCmd.Parameters.Add("@Details", SqlDbType.NVarChar, 500).Value =
                    $"Order #{orderId} placed for Customer {customerId} (Product #{productId}, Qty: {quantity}).";
                await auditCmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return orderId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}