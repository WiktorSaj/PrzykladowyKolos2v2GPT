using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzykładowyKolos2v2.Entities;
[Table("Status")]
public class Status
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<RepairOrder> RepairOrders { get; set; } = [];
}