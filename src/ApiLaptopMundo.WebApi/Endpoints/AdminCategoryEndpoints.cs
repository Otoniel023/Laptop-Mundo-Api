using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.Application.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class AdminCategoryEndpoints
{
    public static void MapAdminCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/categories")
            .WithTags("Admin - Categories")
            .RequireAuthorization();

        // GET /api/admin/categories - List categories for admin
        group.MapGet("/", async ([FromServices] IAdminCategoryService service) =>
        {
            // For now, we reuse the same logic, but we could add more data for admin later
            var categories = await service.GetCategoriesAsync();
            return Results.Ok(categories);
        })
        .WithName("AdminGetCategories")
        .WithDescription("Get a list of all product categories (Admin Access)")
        .WithSummary("List categories (Admin)");

        // POST /api/admin/categories - Create category
        group.MapPost("/", async (
            [FromBody] CreateCategoryDto dto,
            [FromServices] IAdminCategoryService service) =>
        {
            var result = await service.CreateCategoryAsync(dto);
            return Results.Created($"/api/categories/{result.Id}", result);
        })
        .WithName("CreateCategory")
        .WithDescription("Create a new product category (Gaming, Ultrabook, Workstation, etc.)")
        .WithSummary("Create category");

        // PUT /api/admin/categories/{id} - Update category
        group.MapPut("/{id:long}", async (
            long id,
            [FromBody] UpdateCategoryDto dto,
            [FromServices] IAdminCategoryService service) =>
        {
            var result = await service.UpdateCategoryAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateCategory")
        .WithDescription("Update an existing category")
        .WithSummary("Update category");

        // DELETE /api/admin/categories/{id} - Delete category
        group.MapDelete("/{id:long}", async (
            long id,
            [FromServices] IAdminCategoryService service) =>
        {
            var result = await service.DeleteCategoryAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCategory")
        .WithDescription("Delete a category")
        .WithSummary("Delete category");
    }
}
