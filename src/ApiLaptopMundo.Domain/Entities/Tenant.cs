namespace ApiLaptopMundo.Domain.Entities;

public class Tenant
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<TenantProduct> TenantProducts { get; set; } = new List<TenantProduct>();
    public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public ICollection<ShippingMethod> ShippingMethods { get; set; } = new List<ShippingMethod>();
    public ICollection<Tax> Taxes { get; set; } = new List<Tax>();
}
