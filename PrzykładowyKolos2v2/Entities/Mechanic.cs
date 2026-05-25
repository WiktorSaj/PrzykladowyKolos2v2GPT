using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzykładowyKolos2v2.Entities;

[Table("Mechanic")]
public class Mechanic
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; } =  string.Empty;
    [MaxLength(100)]
    public string LastName { get; set; } =  string.Empty;
    
    public ICollection<RepairOrder> RepairOrders { get; set; } = [];
}