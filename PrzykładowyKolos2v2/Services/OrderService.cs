using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using PrzykładowyKolos2v2.Data;
using PrzykładowyKolos2v2.DTOs;
using PrzykładowyKolos2v2.Exceptions;

namespace PrzykładowyKolos2v2.Services;

public class OrderService : IOrderService

{
    
    private readonly AppDbContext _dbContext;
    
    public OrderService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<GetOrderDetailsDto> GetOrderDetailsAsync(int id)
    {
        var order = await _dbContext.RepairOrders
            .Include(x => x.Client)
            .Include(x => x.Mechanic)
            .Include(x => x.Status)
            .Include(x => x.OrderParts)
            .ThenInclude(x => x.Part)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            throw new NotFoundException("Order with id " +  id + " not found" );
        }

        var parts = new List<GetPartDetailsDto>();
        foreach (var item in order.OrderParts)
        {
            var part = new GetPartDetailsDto()
            {
                PartName = item.Part.Name,
                Cost = item.Part.Cost,
                Quantity = item.Quantity
            };
            parts.Add(part);
        }

        var client = new GetClientDetailsDto()
        {
            FullName = order.Client.FullName,
            PhoneNumber = order.Client.PhoneNumber,
        };

        return new GetOrderDetailsDto()
        {
            OrderId = order.Id,
            RegistrationDate = order.RegistrationDate,
            ClosedAt = order.ClosedAt,
            Status = order.Status.Name,
            Client = client,
            Parts = parts
        };


    }

    public async Task UpdateOrderDetailsAsync(int id, UpdateOrderDto dto)
    {
        var order = await _dbContext.RepairOrders.FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            throw new NotFoundException("Order with id " + id + " not found");
        }
        
        var status = await _dbContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.StatusName);

        if (status == null)
        {
            throw new NotFoundException("Status " + dto.StatusName + " not found");
        }

        if (order.ClosedAt.HasValue)
        {
            throw new AlreadyDoneException("Order is already closed");
        }
        
        order.StatusId = status.Id;
        order.ClosedAt = DateTime.Now;
        
        var orderParts = await _dbContext.OrderParts.Where(x => x.OrderId == order.Id).ToListAsync();
        
        _dbContext.OrderParts.RemoveRange(orderParts);
        
        await _dbContext.SaveChangesAsync();
    }
    
    
    // public async Task<List<GetRepairShortDto>> GetRepairsAsync(string? statusName)
    // {
    //     // 1. Tworzymy bazowe zapytanie (IQueryable), jeszcze bez uderzania do bazy danych
    //     var query = _dbContext.RepairOrders
    //         .Include(x => x.Client)
    //         .Include(x => x.Status)
    //         .AsQueryable(); // Pozwala na dynamiczne budowanie zapytania SQL
    //
    //     // 2. Jeśli użytkownik podał status w Query Stringu, doklejamy warunek WHERE
    //     if (!string.IsNullOrEmpty(statusName))
    //     {
    //         query = query.Where(x => x.Status.Name == statusName);
    //     }
    //
    //     // 3. Pobieramy dane z bazy (ToListAsync) i mapujemy na DTO
    //     var orders = await query.ToListAsync();
    //
    //     return orders.Select(x => new GetRepairShortDto
    //     {
    //         OrderId = x.Id,
    //         RegistrationDate = x.RegistrationDate,
    //         Status = x.Status.Name,
    //         ClientFullName = x.Client.FullName
    //     }).ToList();
    // }
}