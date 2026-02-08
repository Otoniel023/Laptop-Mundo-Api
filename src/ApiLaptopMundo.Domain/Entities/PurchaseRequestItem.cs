namespace ApiLaptopMundo.Domain.Entities;

public class PurchaseRequestItem
{
    public long Id { get; set; }
    public long PurchaseRequestId { get; set; }
    public long TenantProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public PurchaseRequest? PurchaseRequest { get; set; }
    public TenantProduct? TenantProduct { get; set; }
}
