using APBD9.DTOs;
using APBD9.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class WarehouseController(IWarehouseService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var idProductWarehouse = await service.AddProductToWarehouseAsync(productWarehouse);
        return Created("", new{idProductWarehouse});
    }
    [HttpPost("procedure")]
    public async Task<IActionResult> AddProductToWarehouseProcedure([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var idProductWarehouse = await service.AddProductToWarehouseProcedureAsync(productWarehouse);
        return Created("", new{idProductWarehouse});
    }
}