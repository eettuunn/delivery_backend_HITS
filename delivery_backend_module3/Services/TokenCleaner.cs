using delivery_backend_module3.Models;
using Microsoft.EntityFrameworkCore;

namespace delivery_backend_module3.Services;

public class TokenCleaner : BackgroundService
{
    private readonly TimeSpan _cleanMinutes;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public TokenCleaner(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _cleanMinutes = TimeSpan.FromMinutes(configuration.GetValue<int>("TokenCleanFrequency"));
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var expiredTokens = await context
                    .Tokens
                    .Where(token => token.ExpiredDate <= DateTime.UtcNow)
                    .ToListAsync(cancellationToken: stoppingToken);

                foreach (var token in expiredTokens)
                {
                    context.Tokens.Remove(token);
                }

                await context.SaveChangesAsync(stoppingToken);
                Console.WriteLine("Cleaned tokens");

                await Task.Delay(_cleanMinutes, stoppingToken);
            }
        }
    }
}