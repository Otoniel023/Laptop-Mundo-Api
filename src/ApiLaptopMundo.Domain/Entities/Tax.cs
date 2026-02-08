namespace ApiLaptopMundo.Domain.Entities;

public class Tax
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public required string Name { get; set; }
    public decimal Rate { get; set; }  // e.g., 0.16 for 16%
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
}
