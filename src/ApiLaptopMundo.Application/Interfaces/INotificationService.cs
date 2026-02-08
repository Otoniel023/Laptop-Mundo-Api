namespace ApiLaptopMundo.Application.Interfaces;

public interface INotificationService
{
    Task SendPurchaseRequestEmailAsync(long purchaseRequestId);
    Task SendPurchaseRequestWhatsAppAsync(long purchaseRequestId);  // Placeholder for future implementation
}
