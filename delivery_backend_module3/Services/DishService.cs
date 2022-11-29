using System.Diagnostics;
using System.Web;
using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Models.Enums;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace delivery_backend_module3.Services;

public class DishService : IDishService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DishService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<DishDto> GetDishDetails(Guid id)
    {
        var dish = await _context
            .Dishes
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (dish == null)
        {
            throw new NotFoundException("can't find dish with this id");
        }

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

        var dishEntity = await _context
            .Dishes
            .Where(x => x.Id == dishId)
            .FirstOrDefaultAsync();
        if (dishEntity == null)
        {
            throw new NotFoundException("can't find dish with this ID");
        }

        var checkRating = await _context
            .Ratings
            .Where(x => x.dish.Id == dishId && x.user.Id == userEntity.Id)
            .FirstOrDefaultAsync();

        if (checkRating != null)
        {
            throw new BadRequestException("User has already rated this dish");
        }

        //TODO: вернуть когда сделаешь заказ
        /*var userOrders = await _context
            .Orders
            .Include(x => x.DishesInBasket)
            .Where(x => x.User.Id == userEntity.Id && x.Status == OrderStatus.Delivered)
            .ToListAsync();
        
        
        if (!CheckDishInOrders(userOrders, dishId))
        {
            throw new BadRequestException("User can't set rating on dish that wasn't ordered");
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

            if (dishEntity == null)
            {
                throw new NotFoundException("Can't find dish with this ID");
            }
            
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

    public async Task<DishPagedListDto> GetDishesList(HttpContext httpContext)
    {
        var query = httpContext.Request.Query;
        int page = 1;
        bool? vegetarian = false;
        DishSorting? sorting = DishSorting.NameAsc;
        List<DishCategory> categories = new();
        ParseQuery(query, ref page, ref vegetarian, ref sorting, ref categories);

        var maxDishesCount = _configuration.GetValue<double>("PageSize");
        var dishCountInDb = _context.Dishes.Count();
        var dishesSkip = (int)((page - 1) * maxDishesCount);
        var takeCount = (int)Math.Min(dishCountInDb - (page - 1) * maxDishesCount, maxDishesCount);
        var pageCount = (int)Math.Ceiling(dishCountInDb / maxDishesCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
        }
        
        var dishEntities = await _context
            .Dishes
            .Where(x => vegetarian == null || x.Vegetarian == vegetarian)
            .Where(x => categories.Count == 0 || categories.Contains(x.Category))
            .Skip(dishesSkip)
            .Take(takeCount)
            .ToListAsync();

        if (sorting != null)
        {
            SortDishes(ref dishEntities, sorting);
        }
        
        var pageInfo = new PageInfoModel
        {
            current = page,
            count = pageCount,
            size = dishEntities.Count
        };

        var dishDtos = new List<DishDto>();

        foreach (var dish in dishEntities)
        {
            dishDtos.Add(await GetMovieElementDto(dish));
        }

        var result = new DishPagedListDto()
        {
            dishes = dishDtos,
            pagination = pageInfo
        };

        return result;
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
    
    private async Task<DishDto> GetMovieElementDto(DishEntity dishEntity)
    {
        var movieElementDto = new DishDto
        {
            id = dishEntity.Id,
            name = dishEntity.Name,
            description = dishEntity.Description,
            price = dishEntity.Price,
            image = dishEntity.Image,
            vegetarian = dishEntity.Vegetarian,
            category = dishEntity.Category.ToString(),
            rating = await GetDishRating(dishEntity)
        };

        return movieElementDto;
    }

    private void ParseQuery(IQueryCollection query, ref int page, ref bool? vegetarian, ref DishSorting? sorting, ref List<DishCategory> categories)
    {
        bool vegetarianFlag = false;
        bool sortingFlag = false;
        foreach (var param in query)
        {
            if (param.Key != "page" && param.Key != "vegetarian" && param.Key != "sorting" && param.Key != "categories")
            {
                throw new BadRequestException("One of query parameter name is incorrect");
            }
            if (param.Key == "page")
            {
                page = Int32.Parse(param.Value);
            }
            if (param.Key == "vegetarian")
            {
                if (param.Value != "true" && param.Value != "false")
                {
                    throw new BadRequestException("Incorrect vegetarian value");
                }
                vegetarian = Boolean.Parse(param.Value);
                vegetarianFlag = true;
            }

            if (param.Key == "sorting")
            {
                if(!Enum.IsDefined(typeof(DishSorting), param.Value.ToString()))
                {
                    throw new BadRequestException("Incorrect sorting value");
                }
                sorting = (DishSorting)Enum.Parse(typeof(DishSorting), param.Value);
                sortingFlag = true;
            }

            if (param.Key == "categories")
            {
                if(!Enum.IsDefined(typeof(DishCategory), param.Value.ToString()))
                {
                    throw new BadRequestException("Incorrect categories value");
                }
                string categoriesStr = param.Value.ToString();
                categoriesStr.Replace("[", "");
                categoriesStr.Replace("\"", "");
                categoriesStr.Replace("]", "");
                List<string> splittedString = categoriesStr.Split(',').ToList();
                foreach (var categorie in splittedString)
                {
                    categories.Add((DishCategory)Enum.Parse(typeof(DishCategory), categorie));
                }
            }
        }

        if (!vegetarianFlag)
        {
            vegetarian = null;
        }

        if (!sortingFlag)
        {
            sorting = null;
        }
    }

    private void SortDishes(ref List<DishEntity> dishEntities, DishSorting? sorting)
    {
        if (sorting == DishSorting.NameAsc)
        {
            Console.WriteLine(sorting);
            dishEntities = dishEntities.OrderBy(x => x.Name).ToList();
        }

        if (sorting == DishSorting.NameDesc)
        {
            dishEntities = dishEntities.OrderByDescending(x => x.Name).ToList();
        }
        if (sorting == DishSorting.PriceAsc)
        {
            dishEntities = dishEntities.OrderBy(x => x.Price).ToList();
        }

        if (sorting == DishSorting.PriceDesc)
        {
            dishEntities = dishEntities.OrderByDescending(x => x.Price).ToList();
        }
        //TODO: rating null sorting
        if (sorting == DishSorting.RatingAsc)
        {
            dishEntities.Sort((x1, x2) =>
            {
                var x1Rating = GetDishRating(x1).Result;
                var x2Rating = GetDishRating(x2).Result;
                if (x2Rating == null)
                {
                    return -1;
                }
                if (x1Rating < x2Rating)
                {
                    return -1;
                }

                return 1;
            });
        }

        if (sorting == DishSorting.RatingDesc)
        {
            dishEntities.Sort((x1, x2) =>
            {
                var x1Rating = GetDishRating(x1).Result;
                var x2Rating = GetDishRating(x2).Result;
                if (x2Rating == null)
                {
                    return -1;
                }
                if (x1Rating < x2Rating)
                {
                    return 1;
                }

                return -1;
            });
        }
    }
}