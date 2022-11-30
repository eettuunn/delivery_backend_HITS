using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;

namespace delivery_backend_module3.Services.Interfaces;

public interface IBasketService
{
    Task AddDishToBasket(Guid id, string email);

    //Task<List<DishBasketDto>> GetUsersBasket(string email);
}