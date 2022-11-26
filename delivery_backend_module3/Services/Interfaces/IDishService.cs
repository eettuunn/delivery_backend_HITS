using delivery_backend_module3.Models.Dtos;

namespace delivery_backend_module3.Services.Interfaces;

public interface IDishService
{
    Task<DishDto> GetDishesDetails(Guid id);

    Task<bool> CheckAbilityToRating(Guid id, string email);
}