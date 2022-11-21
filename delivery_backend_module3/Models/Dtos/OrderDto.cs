using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Dtos;

public class OrderDto
{
    public Guid id { get; set; }
    
    [Required]
    public DateTime deliveryTime { get; set; }
    
    [Required]
    public DateTime orderTime { get; set; }
    
    [Required]
    public OrderStatus status { get; set; }
    
    [Required]
    public double price { get; set; }

    public List<DishDto> dishes { get; set; } = new();
    
    [Required]
    [MinLength(1)]
    public string address { get; set; }
}