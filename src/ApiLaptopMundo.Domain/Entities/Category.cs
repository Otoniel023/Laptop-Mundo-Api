namespace ApiLaptopMundo.Domain.Entities;

public class Category
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
