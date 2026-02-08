using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class PublicProductEndpoints
{
    public static void MapPublicProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public/products")
            .WithTags("Public - Products");

        // GET /api/public/products?tenantId=1 - List products for a specific tenant
        group.MapGet("/", async (
            [FromQuery] long tenantId,
            [FromQuery] long? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? search,
            [FromServices] IProductService productService,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            if (tenantId <= 0)
            {
                return Results.BadRequest(new { error = "tenantId is required" });
            }

            var filter = new ProductFilterDto(
                categoryId,
                minPrice,
                maxPrice,
                search,
                true,  // Only visible products
                page == 0 ? 1 : page,
                pageSize == 0 ? 20 : pageSize
            );

            var result = await productService.GetProductsAsync(tenantId, filter);
            return Results.Ok(result);
        })
        .WithName("GetPublicProducts")
        .WithDescription("Get products for a specific tenant (public access, no authentication required)")
        .WithSummary("List products by tenant")
        .AllowAnonymous();

        // GET /api/public/products/{id}?tenantId=1 - Get product details
        group.MapGet("/{id:long}", async (
            long id,
            [FromQuery] long tenantId,
            [FromServices] IProductService productService) =>
        {
            if (tenantId <= 0)
            {
                return Results.BadRequest(new { error = "tenantId is required" });
            }

            var product = await productService.GetProductByIdAsync(tenantId, id);
            return product != null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetPublicProductById")
        .WithDescription("Get product details for a specific tenant")
        .WithSummary("Get product by ID")
        .AllowAnonymous();

        // GET /api/public/products/featured?tenantId=1 - Get featured products
        group.MapGet("/featured", async (
            [FromQuery] long tenantId,
            [FromServices] IProductService productService) =>
        {
            if (tenantId <= 0)
            {
                return Results.BadRequest(new { error = "tenantId is required" });
            }

            var products = await productService.GetFeaturedProductsAsync(tenantId);
            return Results.Ok(products);
        })
        .WithName("GetPublicFeaturedProducts")
        .WithDescription("Get featured products for a specific tenant")
        .WithSummary("Get featured products")
        .AllowAnonymous();

        // GET /api/public/products/search?tenantId=1&query=laptop - Search products
        group.MapGet("/search", async (
            [FromQuery] long tenantId,
            [FromQuery] string query,
            [FromServices] IProductService productService,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            if (tenantId <= 0)
            {
                return Results.BadRequest(new { error = "tenantId is required" });
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest(new { error = "query is required" });
            }

            var products = await productService.SearchProductsAsync(
                tenantId, 
                query, 
                page == 0 ? 1 : page, 
                pageSize == 0 ? 20 : pageSize);
            return Results.Ok(products);
        })
        .WithName("SearchPublicProducts")
        .WithDescription("Search products for a specific tenant")
        .WithSummary("Search products")
        .AllowAnonymous();
    }
}
