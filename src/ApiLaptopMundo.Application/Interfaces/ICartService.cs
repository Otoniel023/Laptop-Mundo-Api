using ApiLaptopMundo.Application.DTOs.PurchaseRequests;

namespace ApiLaptopMundo.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(long tenantId, long userId);
    Task<CartDto> AddToCartAsync(long tenantId, long userId, AddToCartDto request);
    Task<CartDto> UpdateCartItemAsync(long tenantId, long userId, long tenantProductId, UpdateCartItemDto request);
    Task<CartDto> RemoveFromCartAsync(long tenantId, long userId, long tenantProductId);
    Task<bool> ClearCartAsync(long tenantId, long userId);
    Task<CartDto> ApplyDiscountAsync(long tenantId, long userId, long discountId);
}
