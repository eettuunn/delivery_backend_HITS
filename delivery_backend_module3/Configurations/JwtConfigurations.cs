using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace delivery_backend_module3.Configurations;

public class JwtConfigurations
{
    public const string Issuer = "delivery_backend";                         // издатель 
    public const string Audience = "delivery_frintend";                  // потребитель
    private const string Key = "TheVeryStrongKeyOrPasswordQwerty123";       // ключ для шифрации
    public const int Lifetime = 60;                                          // время жизни токена в минутах
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}