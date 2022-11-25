using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Entities;

public class DishEntity
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Name { get; set; }

    [Required]
    public double Price { get; set; }
    
    public string Description { get; set; }

    public bool Vegetarian { get; set; }

    public string Image { get; set; }

    public DishCategory Category { get; set; }

    public List<UserEntity> WillingUsers { get; set; } = new();

    public List<RatingEntity> Ratings { get; set; } = new();
}