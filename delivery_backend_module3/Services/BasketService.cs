using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
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
    public async Task AddDishToBasket(Guid dishId, string email)
    {
        var user = await _context
            .Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
        DishEntity dish = await _context
            .Dishes
            .Where(x => x.Id == dishId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Cant find dish with this ID");
        DishBasketEntity? dishBasketEntity = await _context
            .DishesInBasket
            .Where(x => x.User == user && x.Dish == dish)
            .FirstOrDefaultAsync();
        
        
        if (dishBasketEntity == null)
        {
            dishBasketEntity = new DishBasketEntity
            {
                Id = new Guid(),
                Dish = dish,
                User = user,
                Amount = 1
            };
            await _context.DishesInBasket.AddAsync(dishBasketEntity);
        }
        else
        {
            dishBasketEntity.Amount++;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<DishBasketDto>> GetUsersBasket(string email)
    {
        var dishes = await _context
            .DishesInBasket
            .Include(x => x.User)
            .Include(x => x.Dish)
            .Where(x => x.User.Email == email)
            .ToListAsync();

        var dishBasketDtos = new List<DishBasketDto>();

        foreach (var dish in dishes)
        {
            DishBasketDto dishBasketDto = new DishBasketDto()
            {
                id = dish.Id,
                name = dish.Dish.Name,
                price = dish.Dish.Price,
                amount = dish.Amount,
                totalPrice = dish.Dish.Price * dish.Amount,
                image = dish.Dish.Image
            };
            dishBasketDtos.Add(dishBasketDto);
        }

        return dishBasketDtos;
    }
}