using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Controllers;

[Route("api/dish")]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<DishDto> GetDishDetails(Guid id)
    {
        return await _dishService.GetDishesDetails(id);
    }

    [HttpGet]
    [Route("{id}/rating/check")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task<bool> CheckAbilityToRating(Guid id)
    {
        return await _dishService.CheckAbilityToRating(id, User.Identity.Name);
    }

    [HttpPost]
    [Route("{id}/rating")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task PostDishReview(Guid id, int rating)
    {
        await _dishService.PostDishRating(id, rating, User.Identity.Name);
    }
}