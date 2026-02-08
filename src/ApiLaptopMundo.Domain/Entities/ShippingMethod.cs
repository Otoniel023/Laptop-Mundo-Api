namespace ApiLaptopMundo.Domain.Entities;

public class ShippingMethod
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public required string Name { get; set; }
    public decimal Cost { get; set; }
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
}
