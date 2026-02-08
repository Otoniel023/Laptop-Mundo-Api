namespace ApiLaptopMundo.Domain.Entities;

public class Discount
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public required string Name { get; set; }
    public required string DiscountType { get; set; }  // "percentage" or "fixed_amount"
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
    public ICollection<PurchaseRequestDiscount> PurchaseRequestDiscounts { get; set; } = new List<PurchaseRequestDiscount>();
}
