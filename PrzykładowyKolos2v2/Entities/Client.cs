using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzykładowyKolos2v2.Entities;
[Table("Client")]
public class Client
{
    [Key]
    public int Id { get; set; }
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    public ICollection<RepairOrder> RepairOrders { get; set; } = [];
}