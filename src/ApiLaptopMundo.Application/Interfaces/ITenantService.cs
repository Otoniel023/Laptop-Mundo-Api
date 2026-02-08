using ApiLaptopMundo.Application.DTOs.Admin;

namespace ApiLaptopMundo.Application.Interfaces;

public interface ITenantService
{
    /// <summary>
    /// Create a new tenant (store)
    /// </summary>
    Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    Task<TenantDto?> GetTenantByIdAsync(long id);

    /// <summary>
    /// Get all tenants
    /// </summary>
    Task<List<TenantDto>> GetTenantsAsync();

    /// <summary>
    /// Update tenant information
    /// </summary>
    Task<TenantDto> UpdateTenantAsync(long id, UpdateTenantDto dto);

    /// <summary>
    /// Delete tenant
    /// </summary>
    Task<bool> DeleteTenantAsync(long id);

    /// <summary>
    /// Create admin user for a tenant
    /// </summary>
    Task<TenantUserDto> CreateTenantAdminAsync(long tenantId, CreateTenantUserDto dto);
}
