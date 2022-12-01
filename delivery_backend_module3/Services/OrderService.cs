using delivery_backend_module3.Exceptions;
using delivery_backend_module3.Models;
using delivery_backend_module3.Models.Dtos;
using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Models.Enums;
using delivery_backend_module3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace delivery_backend_module3.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateOrder(OrderCreateDto orderCreateDto, string email)
    {
        var user = await _context
            .Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync() ?? throw new NotAuthorizedException("Invalid JWT token");
        
        var dishesInBasket = await _context
            .DishesInBasket
            .Include(x => x.User)
            .Include(x => x.Dish)
            .Where(x => x.User.Email == email)
            .ToListAsync();
        if (dishesInBasket.Count == 0)
        {
            throw new ConflictException("No dishes in basket");
        }

        if ((orderCreateDto.deliveryTime - DateTime.Now).TotalMinutes < 60)
        {
            throw new BadRequestException("Delivery time of an order must beat least 60 minutes from now");
        }

        OrderEntity orderEntity = new OrderEntity
        {
            Id = new Guid(),
            DeliveryTime = orderCreateDto.deliveryTime,
            OrderTime = DateTime.UtcNow,
            Address = orderCreateDto.address,
            Status = OrderStatus.InProcess,
            Price = 0,
            User = user,
            DishesInBasket = dishesInBasket
        };
        
        foreach (var dishBasketEntity in dishesInBasket)
        {
            orderEntity.Price += dishBasketEntity.Dish.Price * dishBasketEntity.Amount;
            _context.DishesInBasket.Remove(dishBasketEntity);
        }

        await _context.AddAsync(orderEntity);
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmDelivery(Guid orderId, string email)
    {
        var orderEntity = await _context
            .Orders
            .Include(x => x.User)
            .Where(x => x.Id == orderId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Cant find order with this ID");

        if (orderEntity.User.Email != email)
        {
            throw new ForbiddenException("You cannot confirm not your order");
        }

        if (orderEntity.Status == OrderStatus.Delivered)
        {
            throw new ConflictException("This order already has status 'delivered'");
        }

        orderEntity.Status = OrderStatus.Delivered;
        await _context.SaveChangesAsync();
    }
}