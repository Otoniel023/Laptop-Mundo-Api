using ApiLaptopMundo.Application.DTOs.Products;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Postgrest;

namespace ApiLaptopMundo.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly Supabase.Client _supabaseClient;

    public CategoryService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _supabaseClient
            .From<CategoryModel>()
            .Get();

        return response.Models.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description
        )).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(long categoryId)
    {
        var response = await _supabaseClient
            .From<CategoryModel>()
            .Filter("id", Constants.Operator.Equals, categoryId)
            .Single();

        if (response == null) return null;

        return new CategoryDto(
            response.Id,
            response.Name,
            response.Description
        );
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(long tenantId, long categoryId)
    {
        // Get products in this category
        var productsResponse = await _supabaseClient
            .From<ProductModel>()
            .Filter("category_id", Constants.Operator.Equals, categoryId)
            .Get();

        var productIds = productsResponse.Models.Select(p => p.Id).ToList();

        if (!productIds.Any())
        {
            return new List<ProductDto>();
        }

        // Get tenant products
        var tenantProductsResponse = await _supabaseClient
            .From<TenantProductModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId)
            .Filter("product_id", Constants.Operator.In, productIds)
            .Filter("is_visible", Constants.Operator.Equals, true)
            .Get();

        var tenantProducts = tenantProductsResponse.Models;

        // Get primary images
        var imagesResponse = await _supabaseClient
            .From<ProductImageModel>()
            .Filter("product_id", Constants.Operator.In, productIds)
            .Filter("is_primary", Constants.Operator.Equals, true)
            .Get();

        var primaryImages = imagesResponse.Models
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.First().ImageUrl);

        // Get category name
        var category = await GetCategoryByIdAsync(categoryId);
        var categoryName = category?.Name;

        // Map to DTOs
        var products = new List<ProductDto>();
        foreach (var tp in tenantProducts)
        {
            var product = productsResponse.Models.FirstOrDefault(p => p.Id == tp.ProductId);
            if (product != null)
            {
                products.Add(new ProductDto(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.CategoryId,
                    categoryName,
                    tp.Price,
                    tp.InventoryCount,
                    tp.IsVisible,
                    primaryImages.ContainsKey(product.Id) ? primaryImages[product.Id] : null
                ));
            }
        }

        return products;
    }
}
