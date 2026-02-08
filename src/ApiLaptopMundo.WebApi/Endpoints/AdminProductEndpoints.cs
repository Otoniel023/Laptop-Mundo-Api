using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;
using ApiLaptopMundo.WebApi.Extensions;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class AdminProductEndpoints
{
    public static void MapAdminProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/products")
            .WithTags("Admin - Products")
            .RequireAuthorization(); // TODO: Add admin role check

        // ===== PRODUCT MANAGEMENT =====

        // POST /api/admin/products - Create product
        group.MapPost("/", async (
            [FromBody] CreateProductDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.CreateProductAsync(dto);
            return Results.Created($"/api/products/{result.Id}", result);
        })
        .WithName("CreateProduct")
        .WithDescription("Create a new product (base product without tenant-specific pricing)")
        .WithSummary("Create product");

        // PUT /api/admin/products/{id} - Update product
        group.MapPut("/{id:long}", async (
            long id,
            [FromBody] UpdateProductDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.UpdateProductAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateProduct")
        .WithDescription("Update an existing product")
        .WithSummary("Update product");

        // DELETE /api/admin/products/{id} - Delete product
        group.MapDelete("/{id:long}", async (
            long id,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.DeleteProductAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithDescription("Delete a product")
        .WithSummary("Delete product");

        // ===== PRODUCT VARIANT MANAGEMENT =====

        // POST /api/admin/products/variants - Create variant
        group.MapPost("/variants", async (
            [FromBody] CreateProductVariantDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.CreateProductVariantAsync(dto);
            return Results.Created($"/api/products/{dto.ProductId}/variants", result);
        })
        .WithName("CreateProductVariant")
        .WithDescription("Create a new product variant (color, model, configuration)")
        .WithSummary("Create variant");

        // PUT /api/admin/products/variants/{id} - Update variant
        group.MapPut("/variants/{id:long}", async (
            long id,
            [FromBody] UpdateProductVariantDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.UpdateProductVariantAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateProductVariant")
        .WithDescription("Update an existing product variant")
        .WithSummary("Update variant");

        // DELETE /api/admin/products/variants/{id} - Delete variant
        group.MapDelete("/variants/{id:long}", async (
            long id,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.DeleteProductVariantAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProductVariant")
        .WithDescription("Delete a product variant")
        .WithSummary("Delete variant");

        // ===== PRODUCT SPECIFICATION MANAGEMENT =====

        // POST /api/admin/products/specifications - Create specification
        group.MapPost("/specifications", async (
            [FromBody] CreateProductSpecificationDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.CreateProductSpecificationAsync(dto);
            return Results.Created($"/api/products/{dto.ProductId}/specifications", result);
        })
        .WithName("CreateProductSpecification")
        .WithDescription("Create a new product specification (CPU, GPU, RAM, etc.)")
        .WithSummary("Create specification");

        // PUT /api/admin/products/specifications/{id} - Update specification
        group.MapPut("/specifications/{id:long}", async (
            long id,
            [FromBody] UpdateProductSpecificationDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.UpdateProductSpecificationAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateProductSpecification")
        .WithDescription("Update an existing product specification")
        .WithSummary("Update specification");

        // DELETE /api/admin/products/specifications/{id} - Delete specification
        group.MapDelete("/specifications/{id:long}", async (
            long id,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.DeleteProductSpecificationAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProductSpecification")
        .WithDescription("Delete a product specification")
        .WithSummary("Delete specification");

        // ===== PRODUCT IMAGE MANAGEMENT =====

        // POST /api/admin/products/images - Create image
        group.MapPost("/images", async (
            [FromBody] CreateProductImageDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.CreateProductImageAsync(dto);
            return Results.Created($"/api/products/{dto.ProductId}/images", result);
        })
        .WithName("CreateProductImage")
        .WithDescription("Create a new product image")
        .WithSummary("Create image");

        // PUT /api/admin/products/images/{id} - Update image
        group.MapPut("/images/{id:long}", async (
            long id,
            [FromBody] UpdateProductImageDto dto,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.UpdateProductImageAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateProductImage")
        .WithDescription("Update an existing product image")
        .WithSummary("Update image");

        // DELETE /api/admin/products/images/{id} - Delete image
        group.MapDelete("/images/{id:long}", async (
            long id,
            [FromServices] IAdminProductService service) =>
        {
            var result = await service.DeleteProductImageAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProductImage")
        .WithDescription("Delete a product image")
        .WithSummary("Delete image");

        // POST /api/admin/products/{id}/upload-image - Upload image to storage
        group.MapPost("/{id:long}/upload-image", async (
            long id,
            IFormFile file,
            [FromQuery] bool isPrimary,
            [FromServices] IAdminProductService service) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("No se proporcionó ningún archivo.");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using var stream = file.OpenReadStream();
            
            // 1. Upload to Supabase Storage
            var imageUrl = await service.UploadProductImageAsync(id, fileName, stream, file.ContentType);

            // 2. Save image reference in Database
            var productImage = await service.CreateProductImageAsync(new CreateProductImageDto
            (
                ProductId: id,
                ImageUrl: imageUrl,
                IsPrimary: isPrimary
            ));


            return Results.Ok(productImage);
        })
        .WithName("UploadProductImage")
        .WithDescription("Upload an image file directly to Supabase Storage and link it to the product")
        .WithSummary("Upload product image file")
        .DisableAntiforgery(); // Required for Minimal API form file uploads


        // ===== TENANT PRODUCT MANAGEMENT =====

        // POST /api/admin/products/tenant - Add product to tenant
        group.MapPost("/tenant", async (
            [FromBody] CreateTenantProductDto dto,
            HttpContext context,
            [FromServices] IAdminProductService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.CreateTenantProductAsync(tenantId, dto);
            return Results.Created($"/api/products/{result.Id}", result);
        })
        .WithName("CreateTenantProduct")
        .WithDescription("Add a product to the current tenant with specific pricing and inventory")
        .WithSummary("Add product to tenant");

        // PUT /api/admin/products/tenant/{id} - Update tenant product
        group.MapPut("/tenant/{id:long}", async (
            long id,
            [FromBody] UpdateTenantProductDto dto,
            HttpContext context,
            [FromServices] IAdminProductService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.UpdateTenantProductAsync(tenantId, id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateTenantProduct")
        .WithDescription("Update tenant-specific product pricing and inventory")
        .WithSummary("Update tenant product");

        // DELETE /api/admin/products/tenant/{id} - Remove product from tenant
        group.MapDelete("/tenant/{id:long}", async (
            long id,
            HttpContext context,
            [FromServices] IAdminProductService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.DeleteTenantProductAsync(tenantId, id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTenantProduct")
        .WithDescription("Remove a product from the current tenant")
        .WithSummary("Remove product from tenant");
    }
}
