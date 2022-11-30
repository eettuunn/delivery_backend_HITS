using System.ComponentModel.DataAnnotations;

namespace delivery_backend_module3.Models.Entities;

public class BasketEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public List<DishBasketEntity> DishesInBasket { get; set; } = new();

    [Required] 
    public UserEntity User { get; set; }
}