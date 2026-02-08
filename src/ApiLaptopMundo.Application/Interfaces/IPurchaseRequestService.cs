using ApiLaptopMundo.Application.DTOs.PurchaseRequests;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IPurchaseRequestService
{
    Task<PurchaseRequestDto> CreatePurchaseRequestAsync(long tenantId, long userId, CreatePurchaseRequestDto request);
    Task<List<PurchaseRequestDto>> GetPurchaseRequestsByUserAsync(long tenantId, long userId);
    Task<PurchaseRequestDto?> GetPurchaseRequestByIdAsync(long tenantId, long purchaseRequestId);
    Task<PurchaseRequestDto> UpdateStatusAsync(long tenantId, long purchaseRequestId, UpdatePurchaseRequestStatusDto request);
    Task<bool> CancelPurchaseRequestAsync(long tenantId, long purchaseRequestId);
}
