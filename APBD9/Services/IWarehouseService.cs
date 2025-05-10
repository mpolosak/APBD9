using APBD9.DTOs;

namespace APBD9.Services;

public interface IWarehouseService
{
    public Task<int> AddProductToWarehouseAsync(ProductWarehouseDTO productWarehouse);
    public Task<int> AddProductToWarehouseProcedureAsync(ProductWarehouseDTO productWarehouse);
}