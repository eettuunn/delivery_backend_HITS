using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public string? Address { get; set; }
    
    public string? Email { get; set; }
    
    public string? PhoneNumber { get; set; }

    public List<OrderEntity> Orders { get; set; } = new();

    public List<DishEntity> DishesInBasket { get; set; } = new();
    
    public List<RatingEntity> Ratings { get; set; } = new();
}