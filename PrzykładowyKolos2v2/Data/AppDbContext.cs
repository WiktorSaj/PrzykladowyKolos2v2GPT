using Microsoft.EntityFrameworkCore;
using PrzykładowyKolos2v2.Entities;

namespace PrzykładowyKolos2v2.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<OrderPart> OrderParts { get; set; }
    public DbSet<RepairOrder> RepairOrders { get; set; }
    public DbSet<Part> Parts { get; set; }
}