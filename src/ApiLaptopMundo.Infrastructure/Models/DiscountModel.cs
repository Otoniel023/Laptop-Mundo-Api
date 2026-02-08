using Postgrest.Attributes;
using Postgrest.Models;

namespace ApiLaptopMundo.Infrastructure.Models;

[Table("discounts")]
public class DiscountModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("tenant_id")]
    public long TenantId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("discount_type")]
    public string DiscountType { get; set; } = string.Empty;

    [Column("value")]
    public decimal Value { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
