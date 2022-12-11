using delivery_backend_module3.Models.Dtos;

namespace delivery_backend_module3.Services.Interfaces;

public interface IDishService
{
    Task<DishDto> GetDishDetails(Guid id);

    Task<bool> CheckAbilityToRating(Guid id, string email);

    Task PostDishRating(Guid id, double? rating, string email);

    Task<DishPagedListDto> GetDishesList(HttpContext httpContext);
}