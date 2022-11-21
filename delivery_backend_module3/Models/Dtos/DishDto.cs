using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Models.Dtos;

public class DishDto
{
    public Guid id { get; set; }
    
    [Required]
    [MinLength(1)]
    public string name { get; set; }
    
    public string? description { get; set; }
    
    [Required]
    public double price { get; set; }
    
    public string? image { get; set; }
    
    public bool vegetarian { get; set; }
    
    public double? rating { get; set; }
    
    public DishCategory category { get; set; }
}