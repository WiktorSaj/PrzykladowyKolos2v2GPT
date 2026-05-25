using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PrzykładowyKolos2v2.Entities;
[PrimaryKey(nameof(OrderId),  nameof(PartId))]
[Table("Order_Part")]
public class OrderPart
{
    
    public int OrderId { get; set; }
    public int PartId { get; set; }
    
    public int Quantity { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public RepairOrder Order { get; set; }
    [ForeignKey(nameof(PartId))]
    public Part Part { get; set; }
}