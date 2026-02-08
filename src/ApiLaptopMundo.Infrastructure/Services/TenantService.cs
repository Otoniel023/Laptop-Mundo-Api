using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Supabase;

namespace ApiLaptopMundo.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly Supabase.Client _supabaseClient;

    public TenantService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
    {
        var tenant = new TenantModel
        {
            Name = dto.Name,
            Domain = dto.Domain,
            Description = dto.Description,
            BusinessType = dto.BusinessType,
            IsActive = true,
            Settings = dto.Settings as Dictionary<string, object>
        };

        var response = await _supabaseClient
            .From<TenantModel>()
            .Insert(tenant);

        var created = response.Models.First();

        return MapToDto(created);
    }

    public async Task<TenantDto?> GetTenantByIdAsync(long id)
    {
        var response = await _supabaseClient
            .From<TenantModel>()
            .Where(t => t.Id == id)
            .Get();

        var tenant = response.Models.FirstOrDefault();
        return tenant != null ? MapToDto(tenant) : null;
    }

    public async Task<List<TenantDto>> GetTenantsAsync()
    {
        var response = await _supabaseClient
            .From<TenantModel>()
            .Get();

        return response.Models.Select(MapToDto).ToList();
    }

    public async Task<TenantDto> UpdateTenantAsync(long id, UpdateTenantDto dto)
    {
        var tenant = new TenantModel
        {
            Id = id,
            Name = dto.Name,
            Domain = dto.Domain,
            Description = dto.Description,
            BusinessType = dto.BusinessType,
            IsActive = dto.IsActive,
            Settings = dto.Settings as Dictionary<string, object>
        };

        var response = await _supabaseClient
            .From<TenantModel>()
            .Update(tenant);

        var updated = response.Models.First();
        return MapToDto(updated);
    }

    private TenantDto MapToDto(TenantModel model)
    {
        return new TenantDto(
            model.Id,
            model.Name,
            model.Domain,
            model.Description,
            model.BusinessType,
            model.IsActive,
            model.Settings,
            model.CreatedAt
        );
    }

    public async Task<bool> DeleteTenantAsync(long id)
    {
        await _supabaseClient
            .From<TenantModel>()
            .Where(t => t.Id == id)
            .Delete();

        return true;
    }

    public async Task<TenantUserDto> CreateTenantAdminAsync(long tenantId, CreateTenantUserDto dto)
    {
        // Create user in Supabase Auth with metadata
        var authResponse = await _supabaseClient.Auth.SignUp(
            dto.Email,
            dto.Password,
            new Supabase.Gotrue.SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "tenant_id", tenantId },
                    { "role", dto.Role }
                }
            }
        );

        if (authResponse.User == null)
        {
            throw new Exception("Failed to create user");
        }

        return new TenantUserDto(
            authResponse.User.Id,
            tenantId,
            dto.Email,
            dto.Role,
            DateTime.UtcNow
        );
    }
}
