namespace ApiLaptopMundo.Domain.Entities;

public class PurchaseRequestDiscount
{
    public long Id { get; set; }
    public long PurchaseRequestId { get; set; }
    public long DiscountId { get; set; }
    public decimal DiscountAmount { get; set; }
    
    // Navigation properties
    public PurchaseRequest? PurchaseRequest { get; set; }
    public Discount? Discount { get; set; }
}
