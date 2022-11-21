using System.ComponentModel.DataAnnotations;

namespace delivery_backend_module3.Models.Entities;

public class RatingEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public int rating { get; set; }
    
    [Required]
    public UserEntity user { get; set; }
    
    [Required]
    public DishEntity dish { get; set; }
}