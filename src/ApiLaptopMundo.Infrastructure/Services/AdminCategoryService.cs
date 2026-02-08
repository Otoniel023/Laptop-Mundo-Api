using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.DTOs.Products;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Postgrest;

namespace ApiLaptopMundo.Infrastructure.Services;

public class AdminCategoryService : IAdminCategoryService
{
    private readonly Supabase.Client _supabaseClient;

    public AdminCategoryService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _supabaseClient
            .From<CategoryModel>()
            .Get();

        return response.Models.Select(m => new CategoryDto(
            m.Id,
            m.Name,
            m.Description
        ));
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var model = new CategoryModel
        {
            Name = dto.Name,
            Description = dto.Description
        };

        var response = await _supabaseClient
            .From<CategoryModel>()
            .Insert(model);

        var created = response.Models.First();

        return new CategoryDto(
            created.Id,
            created.Name,
            created.Description
        );
    }

    public async Task<CategoryDto> UpdateCategoryAsync(long categoryId, UpdateCategoryDto dto)
    {
        var model = new CategoryModel
        {
            Id = categoryId,
            Name = dto.Name,
            Description = dto.Description
        };

        var response = await _supabaseClient
            .From<CategoryModel>()
            .Update(model);

        var updated = response.Models.First();

        return new CategoryDto(
            updated.Id,
            updated.Name,
            updated.Description
        );
    }

    public async Task<bool> DeleteCategoryAsync(long categoryId)
    {
        await _supabaseClient
            .From<CategoryModel>()
            .Filter("id", Constants.Operator.Equals, categoryId.ToString())
            .Delete();

        return true;
    }
}
