using ApiLaptopMundo.Application.DTOs.Admin;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Infrastructure.Models;
using Postgrest;

namespace ApiLaptopMundo.Infrastructure.Services;

public class AdminDiscountService : IAdminDiscountService
{
    private readonly Supabase.Client _supabaseClient;

    public AdminDiscountService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<DiscountDto> CreateDiscountAsync(long tenantId, CreateDiscountDto dto)
    {
        var model = new DiscountModel
        {
            TenantId = tenantId,
            Name = dto.Name,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _supabaseClient
            .From<DiscountModel>()
            .Insert(model);

        var created = response.Models.First();

        return new DiscountDto(
            created.Id,
            created.TenantId,
            created.Name,
            created.DiscountType,
            created.Value,
            created.StartDate,
            created.EndDate,
            created.IsActive,
            created.CreatedAt
        );
    }

    public async Task<DiscountDto> UpdateDiscountAsync(long tenantId, long discountId, UpdateDiscountDto dto)
    {
        var model = new DiscountModel
        {
            Id = discountId,
            TenantId = tenantId,
            Name = dto.Name,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive
        };

        var response = await _supabaseClient
            .From<DiscountModel>()
            .Update(model);

        var updated = response.Models.First();

        return new DiscountDto(
            updated.Id,
            updated.TenantId,
            updated.Name,
            updated.DiscountType,
            updated.Value,
            updated.StartDate,
            updated.EndDate,
            updated.IsActive,
            updated.CreatedAt
        );
    }

    public async Task<bool> DeleteDiscountAsync(long tenantId, long discountId)
    {
        await _supabaseClient
            .From<DiscountModel>()
            .Filter("id", Constants.Operator.Equals, discountId.ToString())
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Delete();

        return true;
    }

    public async Task<List<DiscountDto>> GetDiscountsAsync(long tenantId)
    {
        var response = await _supabaseClient
            .From<DiscountModel>()
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Get();

        return response.Models.Select(d => new DiscountDto(
            d.Id,
            d.TenantId,
            d.Name,
            d.DiscountType,
            d.Value,
            d.StartDate,
            d.EndDate,
            d.IsActive,
            d.CreatedAt
        )).ToList();
    }

    public async Task<DiscountDto?> GetDiscountByIdAsync(long tenantId, long discountId)
    {
        var response = await _supabaseClient
            .From<DiscountModel>()
            .Filter("id", Constants.Operator.Equals, discountId.ToString())
            .Filter("tenant_id", Constants.Operator.Equals, tenantId.ToString())
            .Single();

        if (response == null) return null;

        return new DiscountDto(
            response.Id,
            response.TenantId,
            response.Name,
            response.DiscountType,
            response.Value,
            response.StartDate,
            response.EndDate,
            response.IsActive,
            response.CreatedAt
        );
    }
}
