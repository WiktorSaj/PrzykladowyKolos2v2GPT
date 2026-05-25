using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzykładowyKolos2v2.Entities;
[Table("Part")]
public class Part
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "numeric(10,2)")]
    public decimal Cost { get; set; }
    
    public ICollection<OrderPart> OrderParts { get; set; } = [];
}