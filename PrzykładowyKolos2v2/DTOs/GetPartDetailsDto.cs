namespace PrzykładowyKolos2v2.DTOs;

public class GetPartDetailsDto
{
    public string PartName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
}