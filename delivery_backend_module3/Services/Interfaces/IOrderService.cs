using delivery_backend_module3.Models.Dtos;

namespace delivery_backend_module3.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(OrderCreateDto orderCreateDto, string email);

    public Task ConfirmDelivery(Guid orderId, string email);

    public Task<OrderDto> GetOrderInfo(Guid id, string email);

    public Task<List<OrderInfoDto>> GetOrderList(string email);
}