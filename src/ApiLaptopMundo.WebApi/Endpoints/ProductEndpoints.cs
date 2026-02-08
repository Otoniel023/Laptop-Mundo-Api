using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using ApiLaptopMundo.WebApi.Extensions;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        // GET /api/products - List products with filters
        group.MapGet("/", async (
            [FromQuery] long? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? search,
            [FromQuery] long? tenantId,
            HttpContext context,
            [FromServices] IProductService productService,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var effectiveTenantId = tenantId ?? context.GetTenantId();
            
            var filter = new ProductFilterDto(
                categoryId,
                minPrice,
                maxPrice,
                search,
                true,  // Only visible products
                page,
                pageSize
            );

            var result = await productService.GetProductsAsync(effectiveTenantId, filter);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithDescription("Get a paginated list of products with optional filters (category, price range, search query)")
        .WithSummary("List products");

        // GET /api/products/{id} - Get product details
        group.MapGet("/{id:long}", async (
            long id,
            [FromQuery] long? tenantId,
            [FromServices] IProductService productService,
            HttpContext context) =>
        {
            var effectiveTenantId = tenantId ?? context.GetTenantId();
            var product = await productService.GetProductByIdAsync(effectiveTenantId, id);
            
            return product != null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .WithDescription("Get detailed information about a specific product including all its data")
        .WithSummary("Get product details");

        // GET /api/products/{id}/variants - Get product variants
        group.MapGet("/{id:long}/variants", async (
            long id,
            [FromServices] IProductService productService) =>
        {
            var variants = await productService.GetProductVariantsAsync(id);
            return Results.Ok(variants);
        })
        .WithName("GetProductVariants")
        .WithDescription("Get all available variants for a product (different colors, models, configurations)")
        .WithSummary("Get product variants");

        // GET /api/products/{id}/specifications - Get product specifications
        group.MapGet("/{id:long}/specifications", async (
            long id,
            [FromServices] IProductService productService) =>
        {
            var specs = await productService.GetProductSpecificationsAsync(id);
            return Results.Ok(specs);
        })
        .WithName("GetProductSpecifications")
        .WithDescription("Get technical specifications for a product (CPU, GPU, RAM, Storage, Display, etc.)")
        .WithSummary("Get product specifications");

        // GET /api/products/{id}/images - Get product images
        group.MapGet("/{id:long}/images", async (
            long id,
            [FromServices] IProductService productService) =>
        {
            var images = await productService.GetProductImagesAsync(id);
            return Results.Ok(images);
        })
        .WithName("GetProductImages")
        .WithDescription("Get all images for a product including the primary image")
        .WithSummary("Get product images");

        // GET /api/products/featured - Get featured products
        group.MapGet("/featured", async (
            [FromQuery] long? tenantId,
            [FromServices] IProductService productService,
            HttpContext context) =>
        {
            var effectiveTenantId = tenantId ?? context.GetTenantId();
            var products = await productService.GetFeaturedProductsAsync(effectiveTenantId);
            return Results.Ok(products);
        })
        .WithName("GetFeaturedProducts")
        .WithDescription("Get a list of featured/highlighted products for the current tenant")
        .WithSummary("Get featured products");

        // GET /api/products/search - Search products
        group.MapGet("/search", async (
            [FromQuery] string q,
            [FromQuery] long? tenantId,
            HttpContext context,
            [FromServices] IProductService productService,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var effectiveTenantId = tenantId ?? context.GetTenantId();
            var result = await productService.SearchProductsAsync(effectiveTenantId, q, page, pageSize);
            return Results.Ok(result);
        })
        .WithName("SearchProducts")
        .WithDescription("Search products by name or description with pagination")
        .WithSummary("Search products");
    }
}
