using Postgrest.Attributes;
using Postgrest.Models;

namespace ApiLaptopMundo.Infrastructure.Models;

[Table("tenants")]
public class TenantModel : BaseModel
{
    [PrimaryKey("id", false)]  // false = auto-generated, don't include in INSERT
    [Column("id")]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("domain")]
    public string? Domain { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("business_type")]
    public string? BusinessType { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("settings")]
    public Dictionary<string, object>? Settings { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
