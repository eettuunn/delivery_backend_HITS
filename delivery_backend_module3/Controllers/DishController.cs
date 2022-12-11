using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Enums;
using delivery_backend_module3.Services;
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

    /// <summary>
    /// Get list of dishes with parameters
    /// </summary>
    [HttpGet]
    public async Task<DishPagedListDto> GetDishesList(List<DishCategory> categories, bool vegetarian, DishSorting sorting, int page)
    {
        return await _dishService.GetDishesList(HttpContext);
    }

    /// <summary>
    /// Get information about dish
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    public async Task<DishDto> GetDishDetails(Guid id)
    {
        return await _dishService.GetDishDetails(id);
    }

    /// <summary>
    /// Check ability to rating a dish
    /// </summary>
    [HttpGet]
    [Route("{id}/rating/check")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task<bool> CheckAbilityToRating(Guid id)
    {
        return await _dishService.CheckAbilityToRating(id, User.Identity.Name);
    }

    /// <summary>
    /// Post review for a dish
    /// </summary>
    [HttpPost]
    [Route("{id}/rating")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task PostDishReview(Guid id, double? rating)
    {
        await _dishService.PostDishRating(id, rating, User.Identity.Name);
    }
}