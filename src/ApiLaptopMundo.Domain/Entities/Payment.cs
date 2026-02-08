namespace ApiLaptopMundo.Domain.Entities;

public class Payment
{
    public long Id { get; set; }
    public long PurchaseRequestId { get; set; }
    public decimal Amount { get; set; }
    public required string PaymentMethod { get; set; }  // "credit_card", "debit_card", "bank_transfer", "cash"
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public PurchaseRequest? PurchaseRequest { get; set; }
}
