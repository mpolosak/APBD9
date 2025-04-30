using APBD9.DTOs;

namespace APBD9.Services;

public interface IWarehouseService
{
    public Task<int> AddProductToWarehouse(ProductWarehouseDTO productWarehouse);
}