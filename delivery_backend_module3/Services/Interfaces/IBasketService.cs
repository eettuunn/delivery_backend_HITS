namespace delivery_backend_module3.Services.Interfaces;

public interface IBasketService
{
    Task AddDishToBasket(Guid id);
}