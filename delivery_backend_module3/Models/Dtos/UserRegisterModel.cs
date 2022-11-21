using System.ComponentModel.DataAnnotations;
using delivery_backend_module3.Models.Enums;

namespace delivery_backend_module3.Models.Dtos;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public string fullName { get; set; }
    
    [Required]
    [MinLength(6)]
    public string password { get; set; }
    
    [Required]
    [MinLength(1)]
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+")]
    public string email { get; set; }
    
    public string? address { get; set; }
    
    public DateTime? birthDate { get; set; }
    
    public Gender gender { get; set; }
    
    //TODO: регулярка для телефона
    public string phoneNumber { get; set; }
}