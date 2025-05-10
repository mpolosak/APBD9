using System.Data;
using APBD9.DTOs;
using APBD9.Exceptions;
using Microsoft.Data.SqlClient;

namespace APBD9.Services;

public class WarehouseService(IConfiguration configuration) : IWarehouseService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")??throw new ApplicationException("Connection string not found");
    public async Task<int> AddProductToWarehouseAsync(ProductWarehouseDTO productWarehouse)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var transaction = conn.BeginTransaction();
        try
        {
            await using var command = new SqlCommand("SELECT Price FROM Product WHERE IdProduct=@IdProduct", conn, transaction);
            command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            var price = await command.ExecuteScalarAsync();
            if (price is not decimal)
                throw new NotFoundException("Product not found");
            command.CommandText = "SELECT 1 FROM Warehouse WHERE IdWarehouse=@IdWarehouse";
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            if (await command.ExecuteScalarAsync() is null)
                throw new NotFoundException("Warehouse not found");
            command.CommandText = "SELECT IdOrder, CreatedAt FROM [Order] WHERE IdProduct=@IdProduct AND Amount=@Amount";
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            DateTime createdAt;
            int idOrder;
            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                    throw new NotFoundException("Order not found");
                idOrder = reader.GetInt32(0);
                createdAt = reader.GetDateTime(1);
            }
            if(createdAt > productWarehouse.CreatedAt)
                throw new ConflictException("CreatedAt is greater than order createdAt");
            command.CommandText = "SELECT 1 FROM Product_Warehouse WHERE IdOrder=@IdOrder";
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            if(await command.ExecuteScalarAsync() is not null)
                throw new ConflictException("Already exists");
            command.CommandText = "UPDATE [Order] SET FulfilledAt=@FulfilledAt WHERE IdOrder=@IdOrder";
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
            await command.ExecuteNonQueryAsync();
            command.CommandText =
                @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)
                SELECT SCOPE_IDENTITY();";
            command.Parameters.AddWithValue("@Price", productWarehouse.Amount*(decimal) price);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            var id = (decimal) await command.ExecuteScalarAsync();
            await transaction.CommitAsync();
            return (int) id;
        }
        catch (SqlException sqlEx)
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> AddProductToWarehouseProcedureAsync(ProductWarehouseDTO productWarehouse)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("AddProductToWarehouse", conn);
        await conn.OpenAsync();
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdProduct", SqlDbType.Int).Value = productWarehouse.IdProduct;
        command.Parameters.AddWithValue("@IdWarehouse", SqlDbType.Int).Value = productWarehouse.IdWarehouse;
        command.Parameters.AddWithValue("@Amount", SqlDbType.Int).Value = productWarehouse.Amount;
        command.Parameters.AddWithValue("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;
        try
        {
            var id = (decimal)await command.ExecuteScalarAsync();
            return (int)id;
        }
        catch (SqlException sqlEx)
        {
            throw new BadRequestException(sqlEx.Message);
        }
    }
}