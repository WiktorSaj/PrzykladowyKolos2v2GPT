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

PRZYKAŁADOWY GET Z LISTĄ
    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByClientLastNameAsync(string? lastName)
{
    // 1. Pobieramy dane z bazy przy użyciu Eager Loading (Include / ThenInclude)
    var query = _context.Orders
        .Include(o => o.Client) // Potrzebne do filtrowania po nazwisku, jeśli podane
        [span_1](start_span).Include(o => o.OrderPastries) // Łączymy z tabelą pośrednią[span_1](end_span)
            [span_2](start_span).ThenInclude(op => op.Pastry) // Z tabeli pośredniej "wchodzimy" do danych samego wypieku[span_2](end_span)
        .AsQueryable();

    // 2. Filtrowanie (jeśli przekazano nazwisko)
    if (!string.IsNullOrWhiteSpace(lastName))
    {
        query = query.Where(o => o.Client.LastName.ToLower() == lastName.ToLower());
    }

    // Pobieramy encje z bazy danych do pamięci RAM
    var orders = await query.ToListAsync();

    // 3. Mapowanie pobranych encji na obiekty DTO (w pamięci aplikacji)
    return orders.Select(o => new OrderResponseDto
    {
        Id = o.Id,
        AcceptedAt = o.AcceptedAt,
        FulfilledAt = o.FulfilledAt,
        Comments = o.Comments,
        Pastries = o.OrderPastries.Select(op => new OrderPastryResponseDto
        {
            Name = op.Pastry.Name,
            Price = op.Pastry.Price,
            Amount = op.Amount
        }).ToList()
    });



    using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly BakingDbContext _context;

    public OrderService(BakingDbContext context)
    {
        _context = context;
    }

    // ZADANIE 1: Pobieranie zamówień
    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByClientLastNameAsync(string? lastName)
    {
        // Tworzymy bazowe zapytanie do tabeli Orders
        var query = _context.Orders.AsQueryable();

        // Jeśli przesłano nazwisko, filtrujemy po nim (wielkość liter nie ma znaczenia)
        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(o => o.Client.LastName.ToLower() == lastName.ToLower());
        }

        // Mapujemy encje bezpośrednio na DTO, dołączając powiązane 

    // ZADANIE 2: Dodawanie nowego zamówienia (Transakcja)
    public async Task<bool> CreateOrderAsync(int clientId, CreateOrderDto dto)
    {
        // Sprawdzamy, czy klient w ogóle istnieje w bazie
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
        if (!clientExists) return false;

        // Rozpoczynamy transakcję (wymóg z treści zadania)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Tworzymy i dodajemy obiekt główny zamówienia
            var newOrder = new Order
            {
                ClientID = clientId,
                EmployeeID = dto.EmployeeId,
                AcceptedAt = dto.AcceptedAt,
                Comments = dto.Comments
            };

            _context.Orders.Add(newOrder);
            // Zapisujemy, aby wygenerować ID dla nowego zamówienia (potrzebne do tabeli pośredniej)
            await _context.SaveChangesAsync();

            // 2. Przetwarzamy wyroby cukiernicze
            foreach (var pastryDto in dto.Pastries)
            {
                // Szukamy wypieku po nazwie
                var pastry = await _context.Pastries
                    .FirstOrDefaultAsync(p => p.Name == pastryDto.Name);

                // Jeżeli wyrobu nie ma w bazie, wycofujemy wszystko i przerywamy (kod błędu obsłuży kontroler)
                if (pastry == null)
                {
                    await transaction.RollbackAsync();
                    return false; 
                }

                // Tworzymy wpis w tabeli pośredniej Order_Pastry
                var orderPastry = new OrderPastry
                {
                    OrderID = newOrder.Id,
                    PastryID = pastry.Id,
                    Amount = pastryDto.Amount,
                    Comments = pastryDto.Comments
                };

                _context.OrderPastries.Add(orderPastry);
            }

            // Zapisujemy wpisy w tabeli pośredniej
            await _context.SaveChangesAsync();

            // Zatwierdzamy transakcję, jeśli wszystko przebiegło pomyślnie
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            // W razie jakiegokolwiek nieprzewidzianego błędu bazy danych – cofamy zmiany
            await transaction.RollbackAsync();
            throw;
        }
    }
}

}

}
