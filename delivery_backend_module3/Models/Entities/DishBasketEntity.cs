using System.ComponentModel.DataAnnotations;

namespace delivery_backend_module3.Models.Entities;

public class DishBasketEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public OrderEntity Order { get; set; }
    
    [Required]
    public DishEntity Dish { get; set; }

    [Required]
    public int Amount { get; set; }
}