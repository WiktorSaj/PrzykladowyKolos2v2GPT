using Microsoft.AspNetCore.Mvc;
using PrzykładowyKolos2v2.DTOs;
using PrzykładowyKolos2v2.Exceptions;
using PrzykładowyKolos2v2.Services;

namespace PrzykładowyKolos2v2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepairOrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public RepairOrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("/api/repairs/{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        try
        {
            var res = await _orderService.GetOrderDetailsAsync(id);
            return Ok(res);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }


    [HttpPut("/api/repairs/{id}/cancel")]
    public async Task<IActionResult> CancelOrder([FromRoute] int id, [FromBody] UpdateOrderDto dto)
    {
        try
        {
            await _orderService.UpdateOrderDetailsAsync(id, dto);
            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (AlreadyDoneException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // [HttpGet("api/repairs")]
    // public async Task<IActionResult> GetRepairs([FromQuery] string? status)
    // {
    //     // Nie musimy tu dawać try-catch dla NotFound, ponieważ jeśli 
    //     // filtr nic nie znajdzie, poprawne REST API zwraca po prostu pustą listę [] ze statusem 200 OK
    //     var result = await _orderService.GetRepairsAsync(status);
    //     return Ok(result);
    // }
}