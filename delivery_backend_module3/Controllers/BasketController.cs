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

    [HttpPost]
    [Route("{dishId}")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task AddDishToBasket(Guid dishId)
    {
        await _basketService.AddDishToBasket(dishId);
    }
}