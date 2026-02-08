namespace ApiLaptopMundo.Domain.Entities;

public class TenantProduct
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long ProductId { get; set; }
    public decimal Price { get; set; }
    public int InventoryCount { get; set; }
    public bool IsVisible { get; set; } = true;
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
    public Product? Product { get; set; }
    public ICollection<PurchaseRequestItem> PurchaseRequestItems { get; set; } = new List<PurchaseRequestItem>();
}
