using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_module3.Controllers;

[Route("api/account")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost]
    [Route("register")]
    public async Task<TokenDto> RegisterUser([FromBody] UserRegisterModel userRegisterDto)
    {
        return await _usersService.RegisterUser(userRegisterDto);
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost]
    [Route("login")]
    public async Task<TokenDto> LoginUser([FromBody] LoginCredentials loginCredentials)
    {
        return await _usersService.LoginUser(loginCredentials);
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    [Route("logout")]
    public async Task<Response> LogoutUser()
    {
        return await _usersService.LogoutUser(HttpContext);
    }

    /// <summary>
    /// Get information about user's profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    [Route("profile")]
    public async Task<UserDto> GetProfile()
    {
        return await _usersService.GetProfile(User.Identity.Name);
    }
    
    /// <summary>
    /// Edit user's profile
    /// </summary>
    [HttpPut]
    [Route("profile")]
    [Authorize]
    [Authorize(Policy = "ValidateAuthorization")]
    public async Task EditProfile([FromBody] EditUserDto editedUserDto)
    {
        await _usersService.EditProfile(editedUserDto, User.Identity.Name);
    }
}