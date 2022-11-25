using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using delivery_backend_module3.Configurations;
using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace delivery_backend_module3.Services;

public class UsersService : IUsersService
{
    private readonly ApplicationDbContext _context;

    public UsersService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<TokenDto> RegisterUser(UserRegisterModel userRegisterDto)
    {
        
        userRegisterDto.email = NormalizeAttribute(userRegisterDto.email);

        await CheckRegisterValidation(userRegisterDto);

        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            FullName = userRegisterDto.fullName,
            Password = userRegisterDto.password,
            Email = userRegisterDto.email,
            Gender = userRegisterDto.gender,
            BirthDate = userRegisterDto.birthDate,
            Address = userRegisterDto.address,
            PhoneNumber = userRegisterDto.phoneNumber
        };

        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
        
        var loginCredentials = new LoginCredentials
        {
            password = userEntity.Password,
            email = userEntity.Email
        };

        return await LoginUser(loginCredentials);
    }

    public async Task<TokenDto> LoginUser(LoginCredentials loginCredentials)
    {
        loginCredentials.email = NormalizeAttribute(loginCredentials.email);

        var identity = await GetIdentity(loginCredentials.email, loginCredentials.password);

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: JwtConfigurations.Issuer,
            audience: JwtConfigurations.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.AddMinutes(JwtConfigurations.Lifetime),
            signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var result = new TokenDto()
        {
            token = encodeJwt
        };

        return result;
    }

    public async Task<Response> LogoutUser(HttpContext httpContext)
    {
        var token = GetToken(httpContext.Request.Headers);
        
        var handler = new JwtSecurityTokenHandler();
        var expiredDate = handler.ReadJwtToken(token).ValidTo;

        var tokenEntity = new TokenEntity
        {
            Id = Guid.NewGuid(),
            Token = token,
            ExpiredDate = expiredDate
        };

        await _context.Tokens.AddAsync(tokenEntity);
        await _context.SaveChangesAsync();

        var response = new Response()
        {
            status = "OK",
            message = "Logged out"
        };

        return response;
    }
    
    public async Task<UserDto> GetProfile(string email)
    {
        var userEntity = await _context
            .Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
        //TODO: выводится цифра вместо гендера втф да еще и можно 3 поставить ебана
        var user = new UserDto()
        {
            id = userEntity.Id,
            fullName = userEntity.FullName,
            birthDate = userEntity.BirthDate,
            gender = userEntity.Gender,
            address = userEntity.Address,
            email = email,
            phoneNumber = userEntity.PhoneNumber
        };

        return user;
    }

    public async Task EditProfile(EditUserDto editedUserDto, string email)
    {
        var userEntity = await _context
            .Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
        
        CheckPutValidation(editedUserDto);
        
        userEntity.Address = editedUserDto.address;
        userEntity.FullName = editedUserDto.fullName;
        userEntity.PhoneNumber = editedUserDto.phoneNumber;
        userEntity.Gender = editedUserDto.gender;
        userEntity.BirthDate = editedUserDto.birthDate;

        _context.Users.Update(userEntity);
        await _context.SaveChangesAsync(); 
    }

        private static string GetToken(IHeaderDictionary headersDictionary)
    {
        var headers = new Dictionary<string, string>();

        foreach (var header in headersDictionary)
        {
            headers.Add(header.Key, header.Value);
        }

        var authorizationHeader = headers["Authorization"];

        var regex = new Regex(@"\S+\.\S+\.\S+");
        var matches = regex.Matches(authorizationHeader);

        if (matches.Count <= 0)
        {
            throw new BadRequestException("Invalid token in authorization header");
        }

        return matches[0].Value;
    }
    
    private static string NormalizeAttribute(string attribute)
    {
        var result = attribute.ToLower();
        result = result.TrimEnd();

        return result;
    }
    
    private async Task CheckRegisterValidation(UserRegisterModel userRegisterDto)
    {
        var emailRegex = new Regex(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+");
        var emailMatches = emailRegex.Matches(userRegisterDto.email);
        if (emailMatches.Count <= 0)
        {
            throw new BadRequestException("Invalid email");
        }
        
        var checkUniqueEmail = await _context
            .Users
            .Where(x => userRegisterDto.email == x.Email)
            .FirstOrDefaultAsync();

        if (checkUniqueEmail != null)
        {
            throw new UserAlreadyExistException($"Email '{userRegisterDto.email}' is already taken");
        }
        
        if (userRegisterDto.birthDate > DateTime.Now)
        {
            throw new BadRequestException("Birth date must be more then today's date");
        }

        if ((DateTime.Now - userRegisterDto.birthDate).TotalDays / 365 < 7 || (DateTime.Now - userRegisterDto.birthDate).TotalDays / 365 > 99)
        {
            throw new BadRequestException("Your age must be more then 6 and less then 100");
        }
        
        var phoneRegex = new Regex(@"^(\+7|8)(( ?\(\d{3}\) ?\d{3}\-\d{2}\-\d{2})|(\d{10})|( \d{3} \d{3} \d{2} \d{2})|(\-\d{3}\-\d{3}\-\d{2}\-\d{2}))$");
        var phoneMatches = phoneRegex.Matches(userRegisterDto.phoneNumber);
        if (phoneMatches.Count <= 0)
        {
            throw new BadRequestException("Invalid phone number");
        }
    }

    private static void CheckPutValidation(EditUserDto editedUserDto)
    {
        if (editedUserDto.birthDate > DateTime.Now)
        {
            throw new BadRequestException("Birth date must be more then today's date");
        }

        if ((DateTime.Now - editedUserDto.birthDate).TotalDays / 365 < 7 || (DateTime.Now - editedUserDto.birthDate).TotalDays / 365 > 99)
        {
            throw new BadRequestException("Your age must be more then 6 and less then 100");
        }
        
        var phoneRegex = new Regex(@"^(\+7|8)(( ?\(\d{3}\) ?\d{3}\-\d{2}\-\d{2})|(\d{10})|( \d{3} \d{3} \d{2} \d{2})|(\-\d{3}\-\d{3}\-\d{2}\-\d{2}))$");
        var matches = phoneRegex.Matches(editedUserDto.phoneNumber);
        if (matches.Count <= 0)
        {
            throw new BadRequestException("Invalid phone number");
        }
    }
    
    private async Task<ClaimsIdentity> GetIdentity(string email, string password)
    {
        var userEntity = await _context
            .Users
            .Where(x => x.Email == email && x.Password == password)
            .FirstOrDefaultAsync();

        if (userEntity == null)
        {
            throw new WrongLoginCredentialsException("Login failed");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userEntity.Email)
        };

        var claimsIdentity = new ClaimsIdentity
        (
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            "User"
        );

        return claimsIdentity;
    }

}