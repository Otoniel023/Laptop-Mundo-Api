namespace ApiLaptopMundo.Application.DTOs.Products;

public record ProductDto(
    long Id,
    string Name,
    string? Description,
    long? CategoryId,
    string? CategoryName,
    decimal? Price,
    int? InventoryCount,
    bool? IsVisible,
    string? PrimaryImageUrl
);

public record ProductDetailDto(
    long Id,
    string Name,
    string? Description,
    long? CategoryId,
    string? CategoryName,
    decimal? Price,
    int? InventoryCount,
    bool? IsVisible,
    List<ProductVariantDto> Variants,
    List<ProductSpecificationDto> Specifications,
    List<ProductImageDto> Images
);

public record ProductVariantDto(
    long Id,
    long ProductId,
    string Sku,
    string? Size,
    string? Color,
    string? Model,
    decimal Price,
    int InventoryCount,
    bool IsActive
);

public record ProductSpecificationDto(
    long Id,
    long ProductId,
    string Name,
    string Value
);

public record ProductImageDto(
    long Id,
    long ProductId,
    string ImageUrl,
    bool IsPrimary
);

public record ProductFilterDto(
    long? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SearchQuery,
    bool? IsVisible,
    int Page = 1,
    int PageSize = 20
);

public record CategoryDto(
    long Id,
    string Name,
    string? Description
);
