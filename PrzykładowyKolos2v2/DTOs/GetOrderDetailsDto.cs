namespace PrzykładowyKolos2v2.DTOs;

public class GetOrderDetailsDto
{
    public int OrderId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public GetClientDetailsDto Client { get; set; }
    public IEnumerable<GetPartDetailsDto> Parts { get; set; }
}