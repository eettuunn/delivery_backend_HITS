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
    
    /// <summary>
    /// Get list of user's orders
    /// </summary>
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task<List<OrderInfoDto>> GetOrderList()
    {
        return await _orderService.GetOrderList(User.Identity.Name);
    }
    
    /// <summary>
    /// Get information about order
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task<OrderDto> GetOrderInfo(Guid id)
    {
        return await _orderService.GetOrderInfo(id, User.Identity.Name);
    }
    
    /// <summary>
    /// Create new order
    /// </summary>
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task PostCreateOrder([FromBody] OrderCreateDto orderCreateDto)
    {
        await _orderService.CreateOrder(orderCreateDto, User.Identity.Name);
    }
    
    /// <summary>
    /// Confirm delivery of order
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("{id}/status")]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task PostConfirmDelivery(Guid id)
    {
        await _orderService.ConfirmDelivery(id, User.Identity.Name);
    }
}