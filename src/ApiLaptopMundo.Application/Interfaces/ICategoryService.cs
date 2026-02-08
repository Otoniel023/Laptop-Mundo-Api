using ApiLaptopMundo.Application.DTOs.Products;

namespace ApiLaptopMundo.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(long categoryId);
    Task<List<ProductDto>> GetProductsByCategoryAsync(long tenantId, long categoryId);
}
