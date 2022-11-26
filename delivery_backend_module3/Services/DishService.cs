using System.Diagnostics;
using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Models.Enums;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace delivery_backend_module3.Services;

public class DishService : IDishService
{
    private readonly ApplicationDbContext _context;

    public DishService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DishDto> GetDishesDetails(Guid id)
    {
        var dish = await _context
            .Dishes
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        DishDto dishDto = new DishDto()
        {
            id = dish.Id,
            name = dish.Name,
            image = dish.Image,
            description = dish.Description,
            price = dish.Price,
            vegetarian = dish.Vegetarian,
            category = dish.Category.ToString(),
            rating = await GetDishRating(dish)
        };
        return dishDto;
    }

    public async Task<bool> CheckAbilityToRating(Guid dishId, string email)
    {
        var userEntity = await _context
            .Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();

        var checkRating = await _context
            .Ratings
            .Where(x => x.dish.Id == dishId && x.user.Id == userEntity.Id)
            .FirstOrDefaultAsync();

        if (checkRating != null)
        {
            return false;
        }

        //TODO: вернуть когда сделаешь заказ
        /*var userOrders = await _context
            .Orders
            .Include(x => x.DishesInBasket)
            .Where(x => x.User.Id == userEntity.Id && x.Status == OrderStatus.Delivered)
            .ToListAsync();
        
        
        if (!CheckDishInOrders(userOrders, dishId))
        {
            return false;
        }*/
        
        return true;
    }

    public async Task PostDishRating(Guid dishId, int rating, string email)
    {
        if (await CheckAbilityToRating(dishId, email))
        {
            var userEntity = await _context
                .Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();
            var dishEntity = await _context
                .Dishes
                .Where(x => x.Id == dishId)
                .FirstOrDefaultAsync();
            var ratingEntity = new RatingEntity
            {
                Id = new Guid(),
                dish = dishEntity,
                user = userEntity,
                rating = rating
            };

            await _context.Ratings.AddAsync(ratingEntity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException("Cant rating");
        }
    }


    private async Task<double?> GetDishRating(DishEntity dish)
    {
        var ratings = await _context
            .Ratings
            .Where(x => x.dish.Id == dish.Id)
            .ToListAsync();

        if (ratings.Count == 0)
        {
            return null;
        }
        
        double summaryRating = 0;
        
        foreach (var rating in ratings)
        {
            summaryRating += rating.rating;
        }

        var averageRating = summaryRating / ratings.Count;
        
        return averageRating;

    }

    private bool CheckDishInOrders(List<OrderEntity> orders, Guid dishId)
    {
        foreach (var order in orders)
        {
            if (order.DishesInBasket.Exists(x => x.Dish.Id == dishId))
            {
                return true;
            }
        }
        return false;
    }
}