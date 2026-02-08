namespace ApiLaptopMundo.Domain.Entities;

public class ProductVariant
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public required string Sku { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Model { get; set; }
    public decimal Price { get; set; }
    public int InventoryCount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Product? Product { get; set; }
}
