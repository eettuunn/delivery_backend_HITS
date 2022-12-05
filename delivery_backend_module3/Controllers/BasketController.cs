using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Controllers;

[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }
    
    /// <summary>
    /// Get users basket
    /// </summary>
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task<List<DishBasketDto>> GetUsersBasket()
    {
        return await _basketService.GetUsersBasket(User.Identity.Name);
    }

    [HttpPost]
    [Route("dish/{dishId}")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task AddDishToBasket(Guid dishId)
    {
        await _basketService.AddDishToBasket(dishId, User.Identity.Name);
    }

    [HttpDelete]
    [Route("dish/{dishId}")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task DeleteDishFromBasket(Guid dishId, bool? increase = null)
    {
        await _basketService.DeleteDishFromBasket(dishId, increase, HttpContext);
    }
}