using ApiLaptopMundo.Application.DTOs.Products;
using ApiLaptopMundo.Application.DTOs.Common;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResponseDto<ProductDto>> GetProductsAsync(long tenantId, ProductFilterDto filter);
    Task<ProductDetailDto?> GetProductByIdAsync(long tenantId, long productId);
    Task<List<ProductVariantDto>> GetProductVariantsAsync(long productId);
    Task<List<ProductSpecificationDto>> GetProductSpecificationsAsync(long productId);
    Task<List<ProductImageDto>> GetProductImagesAsync(long productId);
    Task<List<ProductDto>> GetFeaturedProductsAsync(long tenantId);
    Task<PaginatedResponseDto<ProductDto>> SearchProductsAsync(long tenantId, string query, int page = 1, int pageSize = 20);
}
