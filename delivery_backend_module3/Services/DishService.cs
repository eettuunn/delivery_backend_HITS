using System.Diagnostics;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;
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
}