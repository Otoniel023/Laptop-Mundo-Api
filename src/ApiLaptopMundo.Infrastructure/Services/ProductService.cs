using ApiLaptopMundo.Application.DTOs.Products;
using ApiLaptopMundo.Application.DTOs.Common;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Postgrest;

namespace ApiLaptopMundo.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly Supabase.Client _supabaseClient;

    public ProductService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<PaginatedResponseDto<ProductDto>> GetProductsAsync(long tenantId, ProductFilterDto filter)
    {
        var query = _supabaseClient
            .From<TenantProductModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Filter("is_visible", Constants.Operator.Equals, "true");

        // Apply filters
        if (filter.MinPrice.HasValue)
        {
            query = query.Filter("price", Constants.Operator.GreaterThanOrEqual, filter.MinPrice.Value.ToString());
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Filter("price", Constants.Operator.LessThanOrEqual, filter.MaxPrice.Value.ToString());
        }

        // Get all results (we'll paginate client-side)
        var response = await query.Get();
        var allTenantProducts = response.Models;
        var totalCount = allTenantProducts.Count;

        // Apply pagination client-side
        var offset = (filter.Page - 1) * filter.PageSize;
        var tenantProducts = allTenantProducts.Skip(offset).Take(filter.PageSize).ToList();

        var productIds = tenantProducts.Select(tp => tp.ProductId).ToList();

        // Get product details
        var products = new List<ProductDto>();
        if (productIds.Any())
        {
            var productsResponse = await _supabaseClient
                .From<ProductModel>()
                .Filter("id", Constants.Operator.In, productIds.Select(id => id.ToString()).ToList())
                .Get();

            var productModels = productsResponse.Models;

            // Get primary images
            var imagesResponse = await _supabaseClient
                .From<ProductImageModel>()
                .Filter("product_id", Constants.Operator.In, productIds.Select(id => id.ToString()).ToList())
                .Filter("is_primary", Constants.Operator.Equals, "true")
                .Get();

            var primaryImages = imagesResponse.Models
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.First().ImageUrl);

            // Get categories
            var categoryIds = productModels.Where(p => p.CategoryId.HasValue).Select(p => p.CategoryId!.Value).Distinct().ToList();
            Dictionary<long, string> categoryNames = new();

            if (categoryIds.Any())
            {
                var categoriesResponse = await _supabaseClient
                    .From<CategoryModel>()
                    .Filter("id", Constants.Operator.In, categoryIds.Select(id => id.ToString()).ToList())
                    .Get();

                categoryNames = categoriesResponse.Models.ToDictionary(c => c.Id, c => c.Name);
            }

            // Map to DTOs
            foreach (var tp in tenantProducts)
            {
                var product = productModels.FirstOrDefault(p => p.Id == tp.ProductId);
                if (product != null)
                {
                    products.Add(new ProductDto(
                        product.Id,
                        product.Name,
                        product.Description,
                        product.CategoryId,
                        product.CategoryId.HasValue && categoryNames.ContainsKey(product.CategoryId.Value) 
                            ? categoryNames[product.CategoryId.Value] 
                            : null,
                        tp.Price,
                        tp.InventoryCount,
                        tp.IsVisible,
                        primaryImages.ContainsKey(product.Id) ? primaryImages[product.Id] : null
                    ));
                }
            }
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        return new PaginatedResponseDto<ProductDto>(
            products,
            filter.Page,
            filter.PageSize,
            totalCount,
            totalPages
        );
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(long tenantId, long productId)
    {
        // Get tenant product
        var tenantProductResponse = await _supabaseClient
            .From<TenantProductModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Filter("product_id", Constants.Operator.Equals, productId.ToString())
            .Single();

        if (tenantProductResponse == null) return null;

        // Get product
        var productResponse = await _supabaseClient
            .From<ProductModel>()
            .Filter("id", Constants.Operator.Equals, productId.ToString())
            .Single();

        if (productResponse == null) return null;

        // Get category name
        string? categoryName = null;
        if (productResponse.CategoryId.HasValue)
        {
            var categoryResponse = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.Equals, productResponse.CategoryId.Value.ToString())
                .Single();

            categoryName = categoryResponse?.Name;
        }

        // Get variants
        var variants = await GetProductVariantsAsync(productId);

        // Get specifications
        var specifications = await GetProductSpecificationsAsync(productId);

        // Get images
        var images = await GetProductImagesAsync(productId);

        return new ProductDetailDto(
            productResponse.Id,
            productResponse.Name,
            productResponse.Description,
            productResponse.CategoryId,
            categoryName,
            tenantProductResponse.Price,
            tenantProductResponse.InventoryCount,
            tenantProductResponse.IsVisible,
            variants,
            specifications,
            images
        );
    }

    public async Task<List<ProductVariantDto>> GetProductVariantsAsync(long productId)
    {
        var response = await _supabaseClient
            .From<ProductVariantModel>()
            .Filter("product_id", Constants.Operator.Equals, productId.ToString())
            .Filter("is_active", Constants.Operator.Equals, "true")
            .Get();

        return response.Models.Select(v => new ProductVariantDto(
            v.Id,
            v.ProductId,
            v.Sku,
            v.Size,
            v.Color,
            v.Model,
            v.Price,
            v.InventoryCount,
            v.IsActive
        )).ToList();
    }

    public async Task<List<ProductSpecificationDto>> GetProductSpecificationsAsync(long productId)
    {
        var response = await _supabaseClient
            .From<ProductSpecificationModel>()
            .Filter("product_id", Constants.Operator.Equals, productId.ToString())
            .Get();

        return response.Models.Select(s => new ProductSpecificationDto(
            s.Id,
            s.ProductId,
            s.Name,
            s.Value
        )).ToList();
    }

    public async Task<List<ProductImageDto>> GetProductImagesAsync(long productId)
    {
        var response = await _supabaseClient
            .From<ProductImageModel>()
            .Filter("product_id", Constants.Operator.Equals, productId.ToString())
            .Get();

        return response.Models.Select(i => new ProductImageDto(
            i.Id,
            i.ProductId,
            i.ImageUrl,
            i.IsPrimary
        )).ToList();
    }

    public async Task<List<ProductDto>> GetFeaturedProductsAsync(long tenantId)
    {
        // For now, just return the first 10 visible products
        // You can add a "is_featured" column to tenant_products table later
        var filter = new ProductFilterDto(
            null,
            null,
            null,
            null,
            true,
            1,
            10
        );

        var result = await GetProductsAsync(tenantId, filter);
        return result.Items;
    }

    public async Task<PaginatedResponseDto<ProductDto>> SearchProductsAsync(long tenantId, string query, int page = 1, int pageSize = 20)
    {
        // Get all tenant products first
        var tenantProductsResponse = await _supabaseClient
            .From<TenantProductModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Filter("is_visible", Constants.Operator.Equals, "true")
            .Get();

        var tenantProducts = tenantProductsResponse.Models;
        var productIds = tenantProducts.Select(tp => tp.ProductId).ToList();

        if (!productIds.Any())
        {
            return new PaginatedResponseDto<ProductDto>(
                new List<ProductDto>(),
                page,
                pageSize,
                0,
                0
            );
        }

        // Search in products by name or description
        var productsResponse = await _supabaseClient
            .From<ProductModel>()
            .Filter("id", Constants.Operator.In, productIds.Select(id => id.ToString()).ToList())
            .Get();

        // Filter by search query (client-side for now)
        var searchLower = query.ToLower();
        var filteredProducts = productsResponse.Models
            .Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                (p.Description != null && p.Description.ToLower().Contains(searchLower)))
            .ToList();

        var totalCount = filteredProducts.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Apply pagination
        var offset = (page - 1) * pageSize;
        var pagedProducts = filteredProducts.Skip(offset).Take(pageSize).ToList();

        // Get primary images
        var pagedProductIds = pagedProducts.Select(p => p.Id).ToList();
        var imagesResponse = await _supabaseClient
            .From<ProductImageModel>()
            .Filter("product_id", Constants.Operator.In, pagedProductIds.Select(id => id.ToString()).ToList())
            .Filter("is_primary", Constants.Operator.Equals, "true")
            .Get();

        var primaryImages = imagesResponse.Models
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.First().ImageUrl);

        // Get categories
        var categoryIds = pagedProducts.Where(p => p.CategoryId.HasValue).Select(p => p.CategoryId!.Value).Distinct().ToList();
        Dictionary<long, string> categoryNames = new();

        if (categoryIds.Any())
        {
            var categoriesResponse = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.In, categoryIds.Select(id => id.ToString()).ToList())
                .Get();

            categoryNames = categoriesResponse.Models.ToDictionary(c => c.Id, c => c.Name);
        }

        // Map to DTOs
        var products = new List<ProductDto>();
        foreach (var product in pagedProducts)
        {
            var tp = tenantProducts.FirstOrDefault(t => t.ProductId == product.Id);
            if (tp != null)
            {
                products.Add(new ProductDto(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.CategoryId,
                    product.CategoryId.HasValue && categoryNames.ContainsKey(product.CategoryId.Value) 
                        ? categoryNames[product.CategoryId.Value] 
                        : null,
                    tp.Price,
                    tp.InventoryCount,
                    tp.IsVisible,
                    primaryImages.ContainsKey(product.Id) ? primaryImages[product.Id] : null
                ));
            }
        }

        return new PaginatedResponseDto<ProductDto>(
            products,
            page,
            pageSize,
            totalCount,
            totalPages
        );
    }
}
