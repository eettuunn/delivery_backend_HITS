using delivery_backend_module3.Models.Entities;
using delivery_backend_module3.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;

namespace delivery_backend_module3.Models;

//[Route("get-dishes")]
//[ApiController]
public class GetDishesFromDB : ControllerBase
{
    private ApplicationDbContext _context;

    public GetDishesFromDB(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task GetDishes()
    {
        var client = new RestClient("https://food-delivery.kreosoft.ru/api");
        var requestString =
            "dish?categories=Wok&categories=Pizza&categories=Soup&categories=Dessert&categories=Drink";
        var request = new RestRequest(requestString);

        var aqq = client.Execute(request).Content;
        dynamic json = JsonConvert.DeserializeObject(aqq);
        int pageCount = Int32.Parse(json["pagination"]["count"].ToString());
        for (var currentPage = 1; currentPage <= pageCount; currentPage++)
        {
            requestString = requestString + $"&page={currentPage}";
            request = new RestRequest(requestString);
            aqq = client.Execute(request).Content;
            json = JsonConvert.DeserializeObject(aqq);
            foreach (var dish in json["dishes"])
            {
                var dishEntity = new DishEntity
                {
                    Id = dish["id"],
                    Name = dish["name"],
                    Price = dish["price"],
                    Description = dish["description"],
                    Vegetarian = dish["vegetarian"],
                    Image = dish["image"],
                    Category = (DishCategory)Enum.Parse(typeof(DishCategory), dish["category"].ToString())
                };
                /*await _context.Dishes.AddAsync(dishEntity);
                await _context.SaveChangesAsync();*/
            }
            requestString = requestString.Replace($"&page={currentPage}", "");
        }
    }
}