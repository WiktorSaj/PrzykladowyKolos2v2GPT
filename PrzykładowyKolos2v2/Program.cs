using Microsoft.EntityFrameworkCore;
using PrzykładowyKolos2v2.Data;
using PrzykładowyKolos2v2.Entities;
using PrzykładowyKolos2v2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();


//Seeding szybki na testy


// Automatyczne dodawanie danych testowych na starcie aplikacji
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Sprawdzamy czy baza jest pusta (np. czy nie ma statusów)
    if (!context.Statuses.Any())
    {
        var testStatus1 = new Status { Name = "In Progress" };
        var testStatus2 = new Status { Name = "Cancelled" };
        context.Statuses.AddRange(testStatus1, testStatus2);

        var testClient = new Client { FullName = "Jan Kowalski", PhoneNumber = "123456789" };
        context.Clients.Add(testClient);

        var testMechanic = new Mechanic { FirstName = "Adam", LastName = "Nowak" };
        context.Mechanics.Add(testMechanic);
        
        // Zapisujemy, aby wygenerować ID potrzebne do kluczy obcych
        context.SaveChanges();

        var testOrder = new RepairOrder
        {
            RegistrationDate = DateTime.Now,
            ClientId = testClient.Id,
            MechanicId = testMechanic.Id,
            StatusId = testStatus1.Id
        };
        context.RepairOrders.Add(testOrder);
        
        context.SaveChanges();
    }
}



app.Run();