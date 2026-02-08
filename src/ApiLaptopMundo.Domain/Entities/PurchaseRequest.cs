namespace ApiLaptopMundo.Domain.Entities;

public class PurchaseRequest
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public required string Status { get; set; }  // "pending", "confirmed", "cancelled", "completed"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
    public User? User { get; set; }
    public ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();
    public ICollection<PurchaseRequestDiscount> Discounts { get; set; } = new List<PurchaseRequestDiscount>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
