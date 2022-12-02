using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Entities;

public class DishBasketEntity
{
    public Guid Id { get; set; }

    [Required]
    public UserEntity User { get; set; }
    
    [Required]
    public DishEntity Dish { get; set; }

    [Required]
    public int Amount { get; set; }
    
    //
    [Required]
    public DishBasketStatus DishStatus { get; set; }
}