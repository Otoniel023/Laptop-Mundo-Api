using Postgrest.Attributes;
using Postgrest.Models;

namespace ApiLaptopMundo.Infrastructure.Models;

[Table("categories")]
public class CategoryModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }
}

[Table("products")]
public class ProductModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_id")]
    public long? CategoryId { get; set; }
}

[Table("product_variants")]
public class ProductVariantModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("sku")]
    public string Sku { get; set; } = string.Empty;

    [Column("size")]
    public string? Size { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("model")]
    public string? Model { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("inventory_count")]
    public int InventoryCount { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("product_specifications")]
public class ProductSpecificationModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("value")]
    public string Value { get; set; } = string.Empty;
}

[Table("product_images")]
public class ProductImageModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("is_primary")]
    public bool IsPrimary { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("tenant_products")]
public class TenantProductModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("tenant_id")]
    public long TenantId { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("inventory_count")]
    public int InventoryCount { get; set; }

    [Column("is_visible")]
    public bool IsVisible { get; set; }
}
