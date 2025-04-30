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
        var idProductWarehouse = await service.AddProductToWarehouse(productWarehouse);
        return Created("", new{idProductWarehouse});
    }
}