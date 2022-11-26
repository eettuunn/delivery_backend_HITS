using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Controllers;

[Route("api/account")]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    [Route("dish/{id}")]
    public async Task<DishDto> GetDishDetails(Guid id)
    {
        return await _dishService.GetDishesDetails(id);
    }
}