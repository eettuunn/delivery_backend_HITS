using System.ComponentModel.DataAnnotations;

namespace delivery_backend_module3.Models.Dtos;

public class OrderCreateDto
{
    [Required]
    public DateTime deliveryTime { get; set; }
    
    // TODO: validation for address
    [Required]
    [MinLength(1)]
    public string address { get; set; }
}