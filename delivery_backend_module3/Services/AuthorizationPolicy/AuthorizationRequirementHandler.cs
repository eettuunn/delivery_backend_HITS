using System.Text.RegularExpressions;
using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace delivery_backend_module3.Services.AuthorizationPolicy;

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public AuthorizationRequirementHandler(IHttpContextAccessor httpContextAccessor,
        IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            var authorizationString = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
            var token = GetToken(authorizationString);

            using var scope = _serviceScopeFactory.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var tokenEntity = await appDbContext
                .Tokens
                .Where(x => x.Token == token)
                .FirstOrDefaultAsync();


            if (tokenEntity != null)
            {
                throw new NotAuthorizedException("Not authorized");
            }

            context.Succeed(requirement);
        }
    }
    
    private static string GetToken(string authorizationString)
    {
        const string pattern = @"\S+\.\S+\.\S+";
        var regex = new Regex(pattern);
        var matches = regex.Matches(authorizationString);

        if (matches.Count <= 0)
        {
            throw new NotAuthorizedException("Not authorized");
        }

        var token = matches[0].Value;

        if (token == null)
        {
            throw new NotAuthorizedException("Not authorized");
        }

        return token;
    }
}