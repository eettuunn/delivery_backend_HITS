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
            .FirstOrDefaultAsync() ?? throw new NotFoundException("can't find dish with this id"); ;

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
            .Include(x => x.user)
            .Include(x => x.dish)
            .Where(x => x.dish.Id == dishId && x.user.Id == userEntity.Id)
            .FirstOrDefaultAsync();

        if (checkRating != null)
        {
            return false;
        }
        
        var userOrders = await _context
            .Orders
            .Include(x => x.Dishes)
            .Include(x => x.User)
            .Where(x => x.User.Id == userEntity.Id && x.Status == OrderStatus.Delivered)
            .ToListAsync();

        bool dishInOrder = await CheckDishInOrders(userOrders, dishId);
        if (!dishInOrder)
        {
            return false;
        }
        
        return true;
    }

    public async Task PostDishRating(Guid dishId, double? rating, string email)
    {
        if (await CheckAbilityToRating(dishId, email))
        {
            CheckRating(rating);
            
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
                rating = Convert.ToInt32(rating)
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

        //TODO: vegetarian
        var dishEntities = await _context
            .Dishes
            .Where(x => vegetarian == false || x.Vegetarian == vegetarian)
            .Where(x => categories.Count == 0 || categories.Contains(x.Category))
            .ToListAsync();
        if (sorting != null)
        {
            SortDishes(ref dishEntities, sorting);
        }
        
        var pageCount = (int)Math.Ceiling(dishEntities.Count / maxDishesCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        dishEntities = dishEntities
            .Skip(dishesSkip)
            .Take(takeCount)
            .ToList();
        
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
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
            dishDtos.Add(await GetDishDetails(dish));
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

    private async Task<bool> CheckDishInOrders(List<OrderEntity> orders, Guid dishId)
    {
        var dishesInBasket = await _context
            .DishesInBasket
            .Include(x => x.Dish)
            .ToListAsync();
        foreach (var order in orders)
        {
            foreach (var dish in order.Dishes)
            {
                Console.WriteLine("?///////////////////////////////");
                Console.WriteLine(dish.Dish);
                Console.WriteLine("?///////////////////////////////");
                if (dish.Dish.Id == dishId)
                {
                    return true;
                }
            }
            /*if (order.Dishes.Exists(x => x.Dish.Id == dishId))
            {
                return true;
            }*/
        }
        return false;
    }
    
    private async Task<DishDto> GetDishDetails(DishEntity dishEntity)
    {
        var dishDto = new DishDto
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

        return dishDto;
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
                if (param.Value == "")
                {
                    throw new BadRequestException("value for query param 'page' can't be empty string");
                }
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
                string categoriesStr = param.Value.ToString();
                categoriesStr.Replace("[", "");
                categoriesStr.Replace("\"", "");
                categoriesStr.Replace("]", "");
                List<string> splittedString = categoriesStr.Split(',').ToList();
                foreach (var categorie in splittedString)
                {
                    if(!Enum.IsDefined(typeof(DishCategory), categorie))
                    {
                        throw new BadRequestException("Incorrect categories value");
                    }
                    Console.WriteLine(categorie);
                    Console.WriteLine("SDAASDASDASDASDASDA");
                    categories.Add((DishCategory)Enum.Parse(typeof(DishCategory), categorie));
                }
            }

        }

        if (!vegetarianFlag)
        {
            vegetarian = false;
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

        if (sorting == DishSorting.RatingAsc)
        {
            var dishesWithRating = dishEntities
                .OrderBy(x => GetDishRating(x).Result)
                .Where(x => GetDishRating(x).Result != null)
                .ToList();
            var dishesWithoutRating = dishEntities
                .Where(x => GetDishRating(x).Result == null)
                .ToList();
            
            dishesWithRating.AddRange(dishesWithoutRating);
            dishEntities = dishesWithRating;
        }

        if (sorting == DishSorting.RatingDesc)
        {
            dishEntities = dishEntities.OrderByDescending(x => GetDishRating(x).Result).ToList();
        }
    }

    private void CheckRating(double? rating)
    {
        if (rating < 0 || rating > 10)
        {
            throw new BadRequestException("Rating must be from 0 to 10");
        }
        if (rating % 1 != 0 || rating == null)
        {
            throw new BadRequestException("Rating must be integer");
        }
    }
}