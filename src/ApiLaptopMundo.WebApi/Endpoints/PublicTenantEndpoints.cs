using ApiLaptopMundo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class PublicTenantEndpoints
{
    public static void MapPublicTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public/tenants")
            .WithTags("Public - Tenants");

        // GET /api/public/tenants/by-domain/{domain} - Get tenant by domain
        group.MapGet("/by-domain/{domain}", async (
            string domain,
            [FromServices] ITenantService service) =>
        {
            var tenants = await service.GetTenantsAsync();
            var tenant = tenants.FirstOrDefault(t => 
                t.Domain != null && t.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase));
            
            return tenant != null ? Results.Ok(tenant) : Results.NotFound();
        })
        .WithName("GetTenantByDomain")
        .WithDescription("Get tenant information by domain (for frontend to identify which tenant to load)")
        .WithSummary("Get tenant by domain")
        .AllowAnonymous();

        // GET /api/public/tenants/by-subdomain/{subdomain} - Get tenant by subdomain
        group.MapGet("/by-subdomain/{subdomain}", async (
            string subdomain,
            [FromServices] ITenantService service) =>
        {
            var tenants = await service.GetTenantsAsync();
            var tenant = tenants.FirstOrDefault(t => 
                t.Domain != null && t.Domain.StartsWith(subdomain + ".", StringComparison.OrdinalIgnoreCase));
            
            return tenant != null ? Results.Ok(tenant) : Results.NotFound();
        })
        .WithName("GetTenantBySubdomain")
        .WithDescription("Get tenant information by subdomain (e.g., 'store1' from 'store1.laptopmundo.com')")
        .WithSummary("Get tenant by subdomain")
        .AllowAnonymous();
    }
}
