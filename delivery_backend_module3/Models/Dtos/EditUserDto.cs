using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Dtos;

public class EditUserDto
{
    [Required]
    [MinLength(1)]
    public string fullName { get; set; }
    
    public DateTime birthDate { get; set; }
    
    [Required]
    public Gender gender { get; set; }
    
    public string address { get; set; }
    
    public string phoneNumber { get; set; }
}