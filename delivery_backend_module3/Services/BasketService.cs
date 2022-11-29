using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace delivery_backend_module3.Services;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContext _context;

    public BasketService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddDishToBasket(Guid dishId)
    {
        DishEntity dish = await _context
            .Dishes
            .Where(x => x.Id == dishId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Can't find dish with this ID");
        
    }
}