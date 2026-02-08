using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.DTOs.Products;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IAdminProductService
{
    // Product Management
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(long productId, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(long productId);

    // Product Variant Management
    Task<ProductVariantDto> CreateProductVariantAsync(CreateProductVariantDto dto);
    Task<ProductVariantDto> UpdateProductVariantAsync(long variantId, UpdateProductVariantDto dto);
    Task<bool> DeleteProductVariantAsync(long variantId);

    // Product Specification Management
    Task<ProductSpecificationDto> CreateProductSpecificationAsync(CreateProductSpecificationDto dto);
    Task<ProductSpecificationDto> UpdateProductSpecificationAsync(long specId, UpdateProductSpecificationDto dto);
    Task<bool> DeleteProductSpecificationAsync(long specId);

    // Product Image Management
    Task<ProductImageDto> CreateProductImageAsync(CreateProductImageDto dto);
    Task<ProductImageDto> UpdateProductImageAsync(long imageId, UpdateProductImageDto dto);
    Task<bool> DeleteProductImageAsync(long imageId);
    Task<string> UploadProductImageAsync(long productId, string fileName, Stream fileStream, string contentType);


    // Tenant Product Management
    Task<ProductDto> CreateTenantProductAsync(long tenantId, CreateTenantProductDto dto);
    Task<ProductDto> UpdateTenantProductAsync(long tenantId, long tenantProductId, UpdateTenantProductDto dto);
    Task<bool> DeleteTenantProductAsync(long tenantId, long tenantProductId);
}
