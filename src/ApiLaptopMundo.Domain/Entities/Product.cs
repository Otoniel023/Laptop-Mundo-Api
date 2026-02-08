namespace ApiLaptopMundo.Domain.Entities;

public class Product
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long? CategoryId { get; set; }
    
    // Navigation properties
    public Category? Category { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<TenantProduct> TenantProducts { get; set; } = new List<TenantProduct>();
}
