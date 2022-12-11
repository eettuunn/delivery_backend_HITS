using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    [Required]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be minimum 6 characters")]
    public string Password { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    public string Address { get; set; }
    
    [Required]
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+")]
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public List<OrderEntity> Orders { get; set; } = new();

    public List<DishEntity> DishesInBasket { get; set; } = new();
    
    public List<RatingEntity> Ratings { get; set; } = new();
}