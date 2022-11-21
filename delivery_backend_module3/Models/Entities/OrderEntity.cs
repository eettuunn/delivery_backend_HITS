using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    [Required]
    public DateTime OrderTime { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Address { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [Required]
    public UserEntity User { get; set; }

    [Required]
    public List<DishBasketEntity> DishesInBasket { get; set; } = new();
}