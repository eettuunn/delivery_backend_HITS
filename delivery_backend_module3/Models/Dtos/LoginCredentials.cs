using System.ComponentModel.DataAnnotations;

namespace delivery_backend_module3.Models.Dtos;

public class LoginCredentials
{
    [Required]
    [MinLength(1)]
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+")]
    public string email { get; set; }
    
    [Required]
    [MinLength(1)]
    public string password { get; set; }
}