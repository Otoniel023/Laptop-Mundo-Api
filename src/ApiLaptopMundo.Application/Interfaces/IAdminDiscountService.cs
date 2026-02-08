using ApiLaptopMundo.Application.DTOs.Admin;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IAdminDiscountService
{
    Task<DiscountDto> CreateDiscountAsync(long tenantId, CreateDiscountDto dto);
    Task<DiscountDto> UpdateDiscountAsync(long tenantId, long discountId, UpdateDiscountDto dto);
    Task<bool> DeleteDiscountAsync(long tenantId, long discountId);
    Task<List<DiscountDto>> GetDiscountsAsync(long tenantId);
    Task<DiscountDto?> GetDiscountByIdAsync(long tenantId, long discountId);
}
