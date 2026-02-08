using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants")
            .WithTags("Tenants");

        // ===== PUBLIC ENDPOINTS (for super admin or initial setup) =====

        // POST /api/tenants - Create new tenant
        group.MapPost("/", async (
            [FromBody] CreateTenantDto dto,
            [FromServices] ITenantService service) =>
        {
            var result = await service.CreateTenantAsync(dto);
            return Results.Created($"/api/tenants/{result.Id}", result);
        })
        .WithName("CreateTenant")
        .WithDescription("Create a new tenant (store). This creates the tenant record in the database.")
        .WithSummary("Create tenant");

        // POST /api/tenants/{id}/admin - Create admin user for tenant
        group.MapPost("/{id:long}/admin", async (
            long id,
            [FromBody] CreateTenantUserDto dto,
            [FromServices] ITenantService service) =>
        {
            var result = await service.CreateTenantAdminAsync(id, dto);
            return Results.Created($"/api/tenants/{id}/users/{result.Id}", result);
        })
        .WithName("CreateTenantAdmin")
        .WithDescription("Create an admin user for a tenant. This user will have access to manage the tenant's data.")
        .WithSummary("Create tenant admin");

        // GET /api/tenants - List all tenants
        group.MapGet("/", async (
            [FromServices] ITenantService service) =>
        {
            var result = await service.GetTenantsAsync();
            return Results.Ok(result);
        })
        .WithName("GetTenants")
        .WithDescription("Get all tenants in the system")
        .WithSummary("List tenants");

        // GET /api/tenants/{id} - Get tenant by ID
        group.MapGet("/{id:long}", async (
            long id,
            [FromServices] ITenantService service) =>
        {
            var result = await service.GetTenantByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetTenant")
        .WithDescription("Get a specific tenant by ID")
        .WithSummary("Get tenant");

        // PUT /api/tenants/{id} - Update tenant
        group.MapPut("/{id:long}", async (
            long id,
            [FromBody] UpdateTenantDto dto,
            [FromServices] ITenantService service) =>
        {
            var result = await service.UpdateTenantAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateTenant")
        .WithDescription("Update tenant information")
        .WithSummary("Update tenant");

        // DELETE /api/tenants/{id} - Delete tenant
        group.MapDelete("/{id:long}", async (
            long id,
            [FromServices] ITenantService service) =>
        {
            var result = await service.DeleteTenantAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTenant")
        .WithDescription("Delete a tenant and all associated data")
        .WithSummary("Delete tenant");
    }
}
