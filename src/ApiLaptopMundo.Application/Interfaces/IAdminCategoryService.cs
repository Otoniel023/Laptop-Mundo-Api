using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.DTOs.Products;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IAdminCategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateCategoryAsync(long categoryId, UpdateCategoryDto dto);
    Task<bool> DeleteCategoryAsync(long categoryId);
}
