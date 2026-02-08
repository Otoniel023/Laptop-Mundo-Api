namespace ApiLaptopMundo.Domain.Entities;

public class ProductSpecification
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public required string Name { get; set; }  // e.g., "CPU", "GPU", "RAM", "Storage", "Display"
    public required string Value { get; set; }  // e.g., "Intel i7-12700H", "RTX 3060", "16GB DDR5"
    
    // Navigation properties
    public Product? Product { get; set; }
}
