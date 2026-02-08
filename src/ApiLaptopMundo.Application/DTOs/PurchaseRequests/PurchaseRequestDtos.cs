namespace ApiLaptopMundo.Application.DTOs.PurchaseRequests;

public record CreatePurchaseRequestDto(
    List<PurchaseRequestItemDto> Items,
    List<long>? DiscountIds
);

public record PurchaseRequestDto(
    long Id,
    long TenantId,
    long UserId,
    string UserEmail,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    List<PurchaseRequestItemDetailDto> Items,
    List<AppliedDiscountDto>? Discounts
);

public record PurchaseRequestItemDto(
    long TenantProductId,
    int Quantity
);

public record PurchaseRequestItemDetailDto(
    long Id,
    long ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    decimal Subtotal
);

public record AppliedDiscountDto(
    long DiscountId,
    string DiscountName,
    decimal DiscountAmount
);

public record UpdatePurchaseRequestStatusDto(
    string Status  // "pending", "confirmed", "cancelled", "completed"
);

public record CartDto(
    List<CartItemDto> Items,
    decimal Subtotal,
    decimal? DiscountAmount,
    decimal Total
);

public record CartItemDto(
    long TenantProductId,
    long ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record AddToCartDto(
    long TenantProductId,
    int Quantity
);

public record UpdateCartItemDto(
    int Quantity
);
