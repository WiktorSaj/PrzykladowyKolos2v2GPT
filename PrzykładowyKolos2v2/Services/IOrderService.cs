using PrzykładowyKolos2v2.DTOs;

namespace PrzykładowyKolos2v2.Services;

public interface IOrderService
{
    Task<GetOrderDetailsDto> GetOrderDetailsAsync(int id);
    
    Task UpdateOrderDetailsAsync(int id, UpdateOrderDto dto);
}