using System.ComponentModel.DataAnnotations.Schema;

namespace PrzykładowyKolos2v2.Entities;

[Table("RepairOrder")]
public class RepairOrder
{
    public int Id { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int ClientId { get; set; }
    public int MechanicId { get; set; }
    public int StatusId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; }
    
    [ForeignKey(nameof(MechanicId))]
    public Mechanic Mechanic { get; set; }
    [ForeignKey(nameof(StatusId))]
    public Status Status { get; set; }


    public ICollection<OrderPart> OrderParts { get; set; } = [];
}