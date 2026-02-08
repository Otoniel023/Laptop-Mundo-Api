using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.DTOs.Products;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Postgrest;
using Supabase.Storage;

namespace ApiLaptopMundo.Infrastructure.Services;

public class AdminProductService : IAdminProductService
{
    private readonly Supabase.Client _supabaseClient;

    public AdminProductService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // Product Management
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var model = new ProductModel
        {
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId
        };

        var response = await _supabaseClient
            .From<ProductModel>()
            .Insert(model);

        var created = response.Models.First();

        // Get category name if exists
        string? categoryName = null;
        if (created.CategoryId.HasValue)
        {
            var category = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.Equals, created.CategoryId.Value.ToString())
                .Single();
            categoryName = category?.Name;
        }

        return new ProductDto(
            created.Id,
            created.Name,
            created.Description,
            created.CategoryId,
            categoryName,
            null, null, null, null
        );
    }

    public async Task<ProductDto> UpdateProductAsync(long productId, UpdateProductDto dto)
    {
        var model = new ProductModel
        {
            Id = productId,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId
        };

        var response = await _supabaseClient
            .From<ProductModel>()
            .Update(model);

        var updated = response.Models.First();

        string? categoryName = null;
        if (updated.CategoryId.HasValue)
        {
            var category = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.Equals, updated.CategoryId.Value.ToString())
                .Single();
            categoryName = category?.Name;
        }

        return new ProductDto(
            updated.Id,
            updated.Name,
            updated.Description,
            updated.CategoryId,
            categoryName,
            null, null, null, null
        );
    }

    public async Task<bool> DeleteProductAsync(long productId)
    {
        await _supabaseClient
            .From<ProductModel>()
            .Filter("id", Constants.Operator.Equals, productId.ToString())
            .Delete();

        return true;
    }

    // Product Variant Management
    public async Task<ProductVariantDto> CreateProductVariantAsync(CreateProductVariantDto dto)
    {
        var model = new ProductVariantModel
        {
            ProductId = dto.ProductId,
            Sku = dto.Sku,
            Size = dto.Size,
            Color = dto.Color,
            Model = dto.Model,
            Price = dto.Price,
            InventoryCount = dto.InventoryCount,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _supabaseClient
            .From<ProductVariantModel>()
            .Insert(model);

        var created = response.Models.First();

        return new ProductVariantDto(
            created.Id,
            created.ProductId,
            created.Sku,
            created.Size,
            created.Color,
            created.Model,
            created.Price,
            created.InventoryCount,
            created.IsActive
        );
    }

    public async Task<ProductVariantDto> UpdateProductVariantAsync(long variantId, UpdateProductVariantDto dto)
    {
        var existing = await _supabaseClient
            .From<ProductVariantModel>()
            .Filter("id", Constants.Operator.Equals, variantId.ToString())
            .Single();

        if (existing == null) throw new Exception("Variant not found");

        existing.Sku = dto.Sku;
        existing.Size = dto.Size;
        existing.Color = dto.Color;
        existing.Model = dto.Model;
        existing.Price = dto.Price;
        existing.InventoryCount = dto.InventoryCount;
        existing.IsActive = dto.IsActive;

        var response = await _supabaseClient
            .From<ProductVariantModel>()
            .Update(existing);

        var updated = response.Models.First();

        return new ProductVariantDto(
            updated.Id,
            updated.ProductId,
            updated.Sku,
            updated.Size,
            updated.Color,
            updated.Model,
            updated.Price,
            updated.InventoryCount,
            updated.IsActive
        );
    }

    public async Task<bool> DeleteProductVariantAsync(long variantId)
    {
        await _supabaseClient
            .From<ProductVariantModel>()
            .Filter("id", Constants.Operator.Equals, variantId.ToString())
            .Delete();

        return true;
    }

    // Product Specification Management
    public async Task<ProductSpecificationDto> CreateProductSpecificationAsync(CreateProductSpecificationDto dto)
    {
        var model = new ProductSpecificationModel
        {
            ProductId = dto.ProductId,
            Name = dto.Name,
            Value = dto.Value
        };

        var response = await _supabaseClient
            .From<ProductSpecificationModel>()
            .Insert(model);

        var created = response.Models.First();

        return new ProductSpecificationDto(
            created.Id,
            created.ProductId,
            created.Name,
            created.Value
        );
    }

    public async Task<ProductSpecificationDto> UpdateProductSpecificationAsync(long specId, UpdateProductSpecificationDto dto)
    {
        var existing = await _supabaseClient
            .From<ProductSpecificationModel>()
            .Filter("id", Constants.Operator.Equals, specId.ToString())
            .Single();

        if (existing == null) throw new Exception("Specification not found");

        existing.Name = dto.Name;
        existing.Value = dto.Value;

        var response = await _supabaseClient
            .From<ProductSpecificationModel>()
            .Update(existing);

        var updated = response.Models.First();

        return new ProductSpecificationDto(
            updated.Id,
            updated.ProductId,
            updated.Name,
            updated.Value
        );
    }

    public async Task<bool> DeleteProductSpecificationAsync(long specId)
    {
        await _supabaseClient
            .From<ProductSpecificationModel>()
            .Filter("id", Constants.Operator.Equals, specId.ToString())
            .Delete();

        return true;
    }

    // Product Image Management
    public async Task<ProductImageDto> CreateProductImageAsync(CreateProductImageDto dto)
    {
        var model = new ProductImageModel
        {
            ProductId = dto.ProductId,
            ImageUrl = dto.ImageUrl,
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _supabaseClient
            .From<ProductImageModel>()
            .Insert(model);

        var created = response.Models.First();

        return new ProductImageDto(
            created.Id,
            created.ProductId,
            created.ImageUrl,
            created.IsPrimary
        );
    }

    public async Task<ProductImageDto> UpdateProductImageAsync(long imageId, UpdateProductImageDto dto)
    {
        var existing = await _supabaseClient
            .From<ProductImageModel>()
            .Filter("id", Constants.Operator.Equals, imageId.ToString())
            .Single();

        if (existing == null) throw new Exception("Image not found");

        existing.ImageUrl = dto.ImageUrl;
        existing.IsPrimary = dto.IsPrimary;

        var response = await _supabaseClient
            .From<ProductImageModel>()
            .Update(existing);

        var updated = response.Models.First();

        return new ProductImageDto(
            updated.Id,
            updated.ProductId,
            updated.ImageUrl,
            updated.IsPrimary
        );
    }

    public async Task<bool> DeleteProductImageAsync(long imageId)
    {
        await _supabaseClient
            .From<ProductImageModel>()
            .Filter("id", Constants.Operator.Equals, imageId.ToString())
            .Delete();

        return true;
    }

    public async Task<string> UploadProductImageAsync(long productId, string fileName, Stream fileStream, string contentType)
    {
        const string bucketName = "laptopmundo";
        
        // 0. Ensure bucket exists
        try 
        {
            await _supabaseClient.Storage.GetBucket(bucketName);
        }
        catch (Exception)
        {
            // If it fails, try to create it (assuming it doesn't exist)
            try {
                await _supabaseClient.Storage.CreateBucket(bucketName, new BucketUpsertOptions { Public = true });
            } catch { /* Ignore if already exists or fails */ }
        }

        // 1. Get reference to bucket and specified path
        var storage = _supabaseClient.Storage.From(bucketName);

        // 2. Define the path inside the bucket: images/{productId}/{fileName}
        var path = $"images/{productId}/{fileName}";

        // 3. Convert Stream to byte array
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var fileData = ms.ToArray();

        // 4. Upload to Supabase Storage
        await storage.Upload(fileData, path, new Supabase.Storage.FileOptions
        {
            ContentType = contentType,
            Upsert = true
        });

        // 5. Return the public URL
        return storage.GetPublicUrl(path);
    }


    // Tenant Product Management
    public async Task<ProductDto> CreateTenantProductAsync(long tenantId, CreateTenantProductDto dto)
    {
        var model = new TenantProductModel
        {
            TenantId = tenantId,
            ProductId = dto.ProductId,
            Price = dto.Price,
            InventoryCount = dto.InventoryCount,
            IsVisible = dto.IsVisible
        };

        var response = await _supabaseClient
            .From<TenantProductModel>()
            .Insert(model);

        var created = response.Models.First();

        // Get product details
        var product = await _supabaseClient
            .From<ProductModel>()
            .Filter("id", Constants.Operator.Equals, created.ProductId.ToString())
            .Single();

        string? categoryName = null;
        if (product?.CategoryId.HasValue == true)
        {
            var category = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.Equals, product.CategoryId.Value.ToString())
                .Single();
            categoryName = category?.Name;
        }

        return new ProductDto(
            product!.Id,
            product.Name,
            product.Description,
            product.CategoryId,
            categoryName,
            created.Price,
            created.InventoryCount,
            created.IsVisible,
            null
        );
    }

    public async Task<ProductDto> UpdateTenantProductAsync(long tenantId, long productId, UpdateTenantProductDto dto)
    {
        // 1. Find the existing mapping
        var existingResponse = await _supabaseClient
            .From<TenantProductModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Filter("product_id", Constants.Operator.Equals, productId.ToString())
            .Single();

        if (existingResponse == null) 
            throw new Exception("Producto no encontrado en este tenant.");

        // 2. Update information
        existingResponse.Price = dto.Price;
        existingResponse.InventoryCount = dto.InventoryCount;
        existingResponse.IsVisible = dto.IsVisible;

        var response = await _supabaseClient
            .From<TenantProductModel>()
            .Update(existingResponse);

        var updated = response.Models.First();

        // Get product details
        var product = await _supabaseClient
            .From<ProductModel>()
            .Filter("id", Constants.Operator.Equals, updated.ProductId.ToString())
            .Single();

        string? categoryName = null;
        if (product?.CategoryId.HasValue == true)
        {
            var category = await _supabaseClient
                .From<CategoryModel>()
                .Filter("id", Constants.Operator.Equals, product.CategoryId.Value.ToString())
                .Single();
            categoryName = category?.Name;
        }

        return new ProductDto(
            product!.Id,
            product.Name,
            product.Description,
            product.CategoryId,
            categoryName,
            updated.Price,
            updated.InventoryCount,
            updated.IsVisible,
            null
        );
    }

    public async Task<bool> DeleteTenantProductAsync(long tenantId, long tenantProductId)
    {
        await _supabaseClient
            .From<TenantProductModel>()
            .Filter("product_id", Constants.Operator.Equals, tenantProductId.ToString())
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Delete();

        return true;
    }
}
