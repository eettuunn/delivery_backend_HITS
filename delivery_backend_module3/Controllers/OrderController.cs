using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Controllers;

[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task PostCreateOrder([FromBody] OrderCreateDto orderCreateDto)
    {
        await _orderService.CreateOrder(orderCreateDto, User.Identity.Name);
    }
}