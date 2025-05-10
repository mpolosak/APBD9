using System.Runtime.CompilerServices;
using APBD9.DTOs;
using APBD9.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APBD9.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class WarehouseController(IWarehouseService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var idProductWarehouse = await service.AddProductToWarehouseAsync(productWarehouse);
        return Created("", new{idProductWarehouse});
    }
    [HttpPost("procedure")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProductToWarehouseProcedure([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var idProductWarehouse = await service.AddProductToWarehouseProcedureAsync(productWarehouse);
        return Created("", new{idProductWarehouse});
    }
}