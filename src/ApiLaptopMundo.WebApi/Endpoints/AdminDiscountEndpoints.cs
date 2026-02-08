using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;
using ApiLaptopMundo.WebApi.Extensions;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class AdminDiscountEndpoints
{
    public static void MapAdminDiscountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/discounts")
            .WithTags("Admin - Discounts")
            .RequireAuthorization(); // TODO: Add admin role check

        // GET /api/admin/discounts - List all discounts
        group.MapGet("/", async (
            HttpContext context,
            [FromServices] IAdminDiscountService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.GetDiscountsAsync(tenantId);
            return Results.Ok(result);
        })
        .WithName("GetDiscounts")
        .WithDescription("Get all discounts for the current tenant")
        .WithSummary("List discounts");

        // GET /api/admin/discounts/{id} - Get discount by ID
        group.MapGet("/{id:long}", async (
            long id,
            HttpContext context,
            [FromServices] IAdminDiscountService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.GetDiscountByIdAsync(tenantId, id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetDiscountById")
        .WithDescription("Get a specific discount by ID")
        .WithSummary("Get discount");

        // POST /api/admin/discounts - Create discount
        group.MapPost("/", async (
            [FromBody] CreateDiscountDto dto,
            HttpContext context,
            [FromServices] IAdminDiscountService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.CreateDiscountAsync(tenantId, dto);
            return Results.Created($"/api/admin/discounts/{result.Id}", result);
        })
        .WithName("CreateDiscount")
        .WithDescription("Create a new discount (percentage or fixed amount)")
        .WithSummary("Create discount");

        // PUT /api/admin/discounts/{id} - Update discount
        group.MapPut("/{id:long}", async (
            long id,
            [FromBody] UpdateDiscountDto dto,
            HttpContext context,
            [FromServices] IAdminDiscountService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.UpdateDiscountAsync(tenantId, id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateDiscount")
        .WithDescription("Update an existing discount")
        .WithSummary("Update discount");

        // DELETE /api/admin/discounts/{id} - Delete discount
        group.MapDelete("/{id:long}", async (
            long id,
            HttpContext context,
            [FromServices] IAdminDiscountService service) =>
        {
            var tenantId = context.GetTenantId();
            var result = await service.DeleteDiscountAsync(tenantId, id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteDiscount")
        .WithDescription("Delete a discount")
        .WithSummary("Delete discount");
    }
}
