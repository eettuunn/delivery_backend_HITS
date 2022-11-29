using delivery_backend_module3.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace delivery_backend_module3.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<DishBasketEntity> DishesInBasket { get; set; }
    
    public DbSet<DishEntity> Dishes { get; set; }
    
    public DbSet<OrderEntity> Orders { get; set; }
    
    public DbSet<RatingEntity> Ratings { get; set; }
    
    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<TokenEntity> Tokens { get; set; }
    
    public DbSet<BasketEntity> Basket { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }
}