namespace ApiLaptopMundo.Application.DTOs.Admin;

// Product Management
public record CreateProductDto(
    string Name,
    string? Description,
    long? CategoryId
);

public record UpdateProductDto(
    string Name,
    string? Description,
    long? CategoryId
);

public record CreateProductVariantDto(
    long ProductId,
    string Sku,
    string? Size,
    string? Color,
    string? Model,
    decimal Price,
    int InventoryCount
);

public record UpdateProductVariantDto(
    string Sku,
    string? Size,
    string? Color,
    string? Model,
    decimal Price,
    int InventoryCount,
    bool IsActive
);

public record CreateProductSpecificationDto(
    long ProductId,
    string Name,
    string Value
);

public record UpdateProductSpecificationDto(
    string Name,
    string Value
);

public record CreateProductImageDto(
    long ProductId,
    string ImageUrl,
    bool IsPrimary
);

public record UpdateProductImageDto(
    string ImageUrl,
    bool IsPrimary
);

// Tenant Product Management
public record CreateTenantProductDto(
    long ProductId,
    decimal Price,
    int InventoryCount,
    bool IsVisible
);

public record UpdateTenantProductDto(
    decimal Price,
    int InventoryCount,
    bool IsVisible
);

// Category Management
public record CreateCategoryDto(
    string Name,
    string? Description
);

public record UpdateCategoryDto(
    string Name,
    string? Description
);

// Discount Management
public record CreateDiscountDto(
    string Name,
    string DiscountType,  // "percentage" or "fixed_amount"
    decimal Value,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive
);

public record UpdateDiscountDto(
    string Name,
    string DiscountType,
    decimal Value,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive
);

public record DiscountDto(
    long Id,
    long TenantId,
    string Name,
    string DiscountType,
    decimal Value,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime CreatedAt
);
